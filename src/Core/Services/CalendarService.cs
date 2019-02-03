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
        private readonly ICalendarDataAccess _calendarData;
        private readonly ClanService _clanService;
        private readonly EventService _eventService;

        public CalendarService(IDiscordMessages discordMessages, ICalendarDataAccess calendarData, ClanService clanService, EventService eventService)
        {
            _discordMessages = discordMessages;
            _calendarData = calendarData;
            _clanService = clanService;
            _eventService = eventService;
        }

        public Task UpdateCalendarsAsync()
        {
            /*
             * Get All guilds
             * Start an update task for every guild
             */

            var clans = _clanService.GetClans().ToList();

            foreach (var clan in clans)
            {
                _= UpdateClanCalendars(clan.Id);
            }

            return Task.CompletedTask;
        }

        public async Task UpdateClanCalendars(int clanId)
        {
            var clan = _clanService.GetClan(clanId);

            var calendars = (await GetClanCalendars(clan.Id)).ToList();
            var events = (await _eventService.GetClanEvents(clan.Id)).ToList();

            var updateTasks = new List<Task>();

            foreach (var calendar in calendars)
            {
                updateTasks.Add(UpdateCalendar(clan.Id, calendar, events));
            }

            try
            {
                await Task.WhenAll(updateTasks.AsEnumerable());
            }
            catch
            {
                var embed = new BotEmbed();
                embed.Description = "Failed to update a calendar.";

                foreach (var update in updateTasks)
                {
                    if (!(update.Exception is null))
                    {
                        Console.WriteLine(update.Exception);
                        _ = _discordMessages.SendMessageAsync(clan.NotificationChannelId, string.Empty, embed);
                    }
                }
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

        public async Task<IEnumerable<Calendar>> GetClanCalendars(int clanId)
        {
            return await _calendarData.GetCalendarsFromGuild(clanId);
        }

        public async Task UpdateCalendar(int clanId, Calendar calendar, IEnumerable<Event> events)
        {
            var embed = CreateCalendarEmbed(calendar, events, calendar.UtcOffset);

            //if message id is 0 the calendar hasn't been posted yet
            if (calendar.MessageId == 0)
            {
                var message = await _discordMessages.SendMessageAsync(calendar.ChannelId, string.Empty, embed);
                await _calendarData.UpdateCalendarMessageId(clanId, calendar.Id, message.MessageId);
            }
            else
            {
                await _discordMessages.EditMessageAsync(calendar.ChannelId, calendar.MessageId, string.Empty, embed);
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
                            startDateTime, endDateTime, @event.Allday, @event.AttendanceDone));
                    }
                }

                days.Add(new Day(dayDate, offsetEvents.AsEnumerable()));
            }

            var embed = new BotEmbed
            {
                Title = $":calendar_spiral: {calendar.Name.ToUpper()}",
                Url = new Uri("https://www.rs-ac.com"), //todo: edit this
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
                    return $"**Today** - {day.Date:MMMM d}```ini{Environment.NewLine} ```";
                }

                return $"**Today** - {day.Date:MMMM d} ```ini{Environment.NewLine}{string.Join(Environment.NewLine + Environment.NewLine, eventString)}```";
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

            if (@event.Allday)
            {
                return $"All day{Environment.NewLine}[{@event.Name} - {@event.LeaderName}]";
            }
            if (@event.StartDateTime.Date.CompareTo(dayDate.Date) < 0)
            {
                //end in future
                if (@event.EndDateTime.Date.CompareTo(dayDate.Date) > 0)
                {
                    return $"All day{Environment.NewLine}[{@event.Name} - {@event.LeaderName}]";
                }
                //ends today
                else
                {
                    return $"Ends at {@event.EndDateTime.ToString("t")}{Environment.NewLine}[{@event.Name} - {@event.LeaderName}]";
                }
            }
            else
            {
                //end in future
                if (@event.EndDateTime.Date.CompareTo(dayDate.Date) > 0)
                {
                    return $"{@event.StartDateTime.ToString("t")} - {@event.EndDateTime.ToString("M")}{Environment.NewLine}[{@event.Name} - {@event.LeaderName}]";
                }
                //ends today
                else
                {
                    return $"{@event.StartDateTime.ToString("t")} - {@event.EndDateTime.ToString("t")}{Environment.NewLine}[{@event.Name} - {@event.LeaderName}]";
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