using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Entities.Calendars;
using Core.Storage;

namespace Core.Services
{
    public class CalendarService
    {
        private readonly IDiscordMessages _discordMessages;
        private readonly IDiscordGuilds _discordGuilds;
        private readonly ICalendarDataAccess _calendarData;
        private readonly IDiscordMembers _discordMembers;

        public CalendarService(IDiscordMessages discordMessages, ICalendarDataAccess calendarData, IDiscordGuilds discordGuilds, IDiscordMembers discordMembers)
        {
            _discordMessages = discordMessages;
            _calendarData = calendarData;
            _discordGuilds = discordGuilds;
            _discordMembers = discordMembers;
        }

        public async Task UpdateCalendarsAsync()
        {
            /*
             * Get All guilds
             * Start an update task for every guild
             */

            var guilds = await _discordGuilds.GetGuildsAsync();

            var updates = new List<Task<IEnumerable<Task>>>();

            foreach (var botGuild in guilds)
            {
                updates.Add(UpdateGuildCalendars(botGuild));
            }
        }

        //public async Task CreateCalendarAsync(BotGuild guild, BotChannel channel, int utcOffset)
        //{
        //    var events = new Dictionary<Event, BotMember>();

        //    var leaders = (await GetEventLeadersAsync(guild.GuildId, guild.GetEvents())).ToList();

        //    foreach (var @event in guild.GetEvents())
        //    {
        //        var leader = leaders.FirstOrDefault(x => x.Id == @event.LeaderDiscordId);
        //        events.Add(@event, leader);
        //    }

        //    var embed = CreateCalendarEmbed(events, utcOffset);
        //    var message = await _discordMessages.SendMessageAsync(channel, string.Empty, embed);
        //    var calendar = new Calendar(message, utcOffset);
        //    await _calendarData.AddCalendar(guild.GuildId, calendar);
        //}

        public async Task<IEnumerable<Task>> UpdateGuildCalendars(BotGuild guild)
        {
            /*
             * Update all guilds calendars
             */

            var updateTasks = new List<Task>();

            foreach (var calendar in guild.GetCalendars())
            {
                updateTasks.Add(UpdateCalendar(calendar, guild.GetEvents()));
            }

            try
            {
                await Task.WhenAll(updateTasks.AsEnumerable());
            }
            catch
            {
                foreach (var update in updateTasks)
                {
                    if (!(update.Exception is null))
                    {
                        //todo: handle exceptions
                        Console.WriteLine(update.Exception);
                    }
                }
            }

            return updateTasks.AsEnumerable();
        }

        private async Task UpdateCalendar(Calendar calendar, IEnumerable<Event> events)
        {
            var embed = CreateCalendarEmbed(calendar, events, calendar.UtcOffset);

            //if message id is 0 the calendar hasn't been posted yet
            if (calendar.MessageId == 0)
            {
                var message = await _discordMessages.SendMessageAsync(calendar.ChannelId, string.Empty, embed);
                await _calendarData.UpdateCalendarMessageId(calendar.Id, message.MessageId);
            }
            else
            {
                await _discordMessages.EditMessageAsync(new BotMessage(calendar.MessageId, calendar.ChannelId), string.Empty, embed);
            }
        }

        private BotEmbed CreateCalendarEmbed(Calendar calendar, IEnumerable<Event> events, int utcOffset)
        {
            var now = DateTime.UtcNow.AddHours(utcOffset);

            var timezone = string.Empty;
            if (utcOffset < 0)
            {
                timezone = utcOffset.ToString();
            }
            else
            {
                timezone = $"+{utcOffset}";
            }

            var days = new List<Day>();
            for (int i = 0; i < 7; i++)
            {
                var dayDate = now.Date.AddDays(i);

                var offsetEvents = new List<Event>();

                foreach (var @event in events)
                {

                    var startDateTime = @event.StartDateTime.ToUniversalTime().AddHours(utcOffset);
                    var endDateTime = @event.EndDateTime.ToUniversalTime().AddHours(utcOffset);

                    //add event to day
                    if (dayDate.CompareTo(startDateTime.Date) >= 0 && dayDate.CompareTo(endDateTime.Date) <= 0)
                    {
                        offsetEvents.Add(new Event(@event.Id, @event.LeaderName, @event.Name, @event.Active,
                            startDateTime, @event.EndDateTime.ToUniversalTime().AddHours(utcOffset)));
                    }
                }

                days.Add(new Day(dayDate, offsetEvents.AsEnumerable()));
            }

            var embed = new BotEmbed
            {
                Title = $":calendar_spiral: {calendar.Name.ToUpper()}",
                Url = new Uri("https://www.rs-ac.com"),
                Footer = new BotFooter(
                    $"Last synched with the clan's calendar on {now.ToString("dddd MMMM d")} at {now.ToString("t")} (UTC{timezone})."),
                ColorHex = calendar.ColorHex,
                Description = FormatCalendar(days, timezone, now)
            };

            return embed;
        }

        private string FormatCalendar(IEnumerable<Day> days, string utcOffset, DateTime dateTimeNow)
        {
            var offsetInfo = $"All shown times are UTC{utcOffset}.";
            var calendarContent = new List<string>();
            var completed = false;
            var contentLength = 0;
            foreach (var day in days)
            {
                var dayFormatted = FormatDay(day, dateTimeNow);
                if (contentLength + dayFormatted.Length <= 1900)
                {
                    calendarContent.Add(dayFormatted);
                    contentLength += dayFormatted.Length;
                }
                else
                {
                    break;
                }
            }

            return $"{offsetInfo}{Environment.NewLine + Environment.NewLine}{string.Join(Environment.NewLine, calendarContent)}";
        }

        private string FormatDay(Day day, DateTime dateTimeNow)
        {
            //format events
            var eventString = new List<string>();
            var descriptionLength = 0;

            foreach (var @event in day.Events)
            {
                var content = FormatEvent(@event, day.Date);
                if (content.Length + descriptionLength >= 1900) break;
                eventString.Add(content);
                descriptionLength += content.Length;
            }

            if (day.Date.Date.CompareTo(dateTimeNow.Date) == 0)
            {
                if (day.Events == null | eventString.Count < 1)
                {
                    return $"**Today**```ini{Environment.NewLine} ```";
                }

                return $"**Today**```ini{Environment.NewLine}{string.Join(Environment.NewLine + Environment.NewLine, eventString)}```";
            }
            else
            {
                if (day.Events == null | eventString.Count < 1)
                {
                    return $"**{day.Date:dddd}** - {day.Date:MMMM d}```ini{Environment.NewLine} ```";
                }

                return $"**{day.Date:dddd}** - {day.Date:MMMM d}```ini{Environment.NewLine}{string.Join(Environment.NewLine + Environment.NewLine, eventString)}```";
            }
        }

        private string FormatEvent(Event @event, DateTime dayDate)
        {
            //todo: implement all day events if needed
            //started
            if (@event.StartDateTime.Date.CompareTo(dayDate.Date) < 0)
            {
                //end in future
                if (@event.EndDateTime.Date.CompareTo(dayDate.Date) > 0)
                {
                    return $"Ends on {@event.EndDateTime.ToString("M")}{Environment.NewLine}[{@event.Id}] [{@event.Name} - {@event.LeaderName}]";
                }
                //ends today
                else
                {
                    return $"Ends at {@event.EndDateTime.ToString("t")}{Environment.NewLine}[{@event.Id}] [{@event.Name} - {@event.LeaderName}]";
                }
            }
            else
            {
                //end in future
                if (@event.EndDateTime.Date.CompareTo(dayDate.Date) > 0)
                {
                    return $"{@event.StartDateTime.ToString("t")} - {@event.EndDateTime.ToString("M")}{Environment.NewLine}[{@event.Id}] [{@event.Name} - {@event.LeaderName}]";
                }
                //ends today
                else
                {
                    return $"{@event.StartDateTime.ToString("t")} - {@event.EndDateTime.ToString("t")}{Environment.NewLine}[{@event.Id}] [{@event.Name} - {@event.LeaderName}]";
                }
            }
        }

        //private async Task<IEnumerable<BotMember>> GetEventLeadersAsync(ulong guildId, IEnumerable<Event> events)
        //{
        //    var leaders = new List<BotMember>();
        //    var leadersNotFound = new List<ulong>();
        //    foreach (var @event in events)
        //    {
        //        var found = false;

        //        if(leadersNotFound.Contains(@event.LeaderDiscordId)) continue;

        //        foreach (var botMember in leaders)
        //        {
        //            if (botMember.Id == @event.LeaderDiscordId)
        //            {
        //                found = true;
        //                break;
        //            }
        //        }

        //        if (found) continue;
        //        try
        //        {
        //            leaders.Add(await _discordMembers.GetBotGuildMember(guildId, @event.LeaderDiscordId));
        //        }
        //        catch
        //        {
        //            leadersNotFound.Add(@event.LeaderDiscordId);
        //        }
        //    }

        //    return leaders.AsEnumerable();
        //}
    }
}