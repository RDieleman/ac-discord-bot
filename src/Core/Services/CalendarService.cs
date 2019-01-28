using System;
using System.Collections.Generic;
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
        private readonly IDiscordMessages _discord;
        private readonly ICalendarDataAccess _calendarData;
        private readonly GuildService _guildService;

        public CalendarService(IDiscordMessages discord, ICalendarDataAccess calendarData, GuildService guildService)
        {
            _discord = discord;
            _calendarData = calendarData;
            _guildService = guildService;
        }

        public async Task UpdateCalendarsAsync()
        {
            /*
             * Get All guilds
             * Start an update task for every guild
             */

            var guilds = await _guildService.GetAllGuildsAsync();

            var updates = new List<Task<IEnumerable<Task>>>();

            foreach (var botGuild in guilds)
            {
                updates.Add(UpdateGuildCalendars(botGuild));
            }
        }


        public async Task CreateCalendarAsync(BotChannel channel, int utcOffset)
        {
            var guild = await _guildService.GetGuildAsync(channel.GuildId);
            var embed = CreateCalendarEmbed(guild.GetEvents(), utcOffset);
            var message = await _discord.SendMessageAsync(channel, string.Empty, embed);
            var calendar = new Calendar(message, utcOffset);
            await _calendarData.AddCalendar(calendar);
        }

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
                    }
                }
            }

            return updateTasks.AsEnumerable();
        }

        private async Task UpdateCalendar(Calendar calendar, IEnumerable<Event> events)
        {
            var embed = CreateCalendarEmbed(events, calendar.UtcOffset);
            await _discord.EditMessageAsync(calendar.Message, string.Empty, embed);
        }

        private BotEmbed CreateCalendarEmbed(IEnumerable<Event> events, int utcOffset)
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

                    //add event to day
                    if (dayDate.Equals(startDateTime.Date))
                    {
                        offsetEvents.Add(new Event(@event.Id, @event.Leader, @event.Name,
                            startDateTime, @event.EndDateTime.ToUniversalTime().AddHours(utcOffset)));
                    }
                }

                days.Add(new Day(dayDate, offsetEvents.AsEnumerable()));
            }

            var embed = new BotEmbed
            {
                Title = $":calendar_spiral: CLAN CALENDAR",
                Footer = new BotFooter(
                    $"Last synched with the clan's calendar on {now.ToString("dddd MMMM d")} at {now.ToString("t")} (UTC{timezone})."),
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
                if (contentLength + dayFormatted.Length <= 1800)
                {
                    calendarContent.Add(dayFormatted);
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
            day.Events.ToList().ForEach(x =>
            {
                eventString.Add(FormatEvent(x, day.Date));
            });

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
                    return $"Ends on {@event.EndDateTime.ToString("M")}{Environment.NewLine}[{@event.Name} - {@event.Leader.Username}]";
                }
                //ends today
                else
                {
                    return $"Ends at {@event.EndDateTime.ToString("t")}{Environment.NewLine}[{@event.Name} - {@event.Leader.Username}]";
                }
            }
            else
            {
                //end in future
                if (@event.EndDateTime.Date.CompareTo(dayDate.Date) > 0)
                {
                    return $"{@event.StartDateTime.ToString("t")} - {@event.EndDateTime.ToString("M")}{Environment.NewLine}[{@event.Name} - {@event.Leader.Username}]";
                }
                //ends today
                else
                {
                    return $"{@event.StartDateTime.ToString("t")} - {@event.EndDateTime.ToString("t")}{Environment.NewLine}[{@event.Name} - {@event.Leader.Username}]";
                }
            }
        }
    }
}