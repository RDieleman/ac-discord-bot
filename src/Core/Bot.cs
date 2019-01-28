using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities.Timers;
using Core.Services;
using Core.Storage;
using Discord.Entities;

namespace Core
{
    public class Bot
    {
        private readonly IDiscord _discord;
        private readonly IDiscordMessages _discordMessages;
        private readonly IGuildDataAccess _guildData;
        private readonly ICalendarDataAccess _calendarData;

        private readonly List<ITimer> _timers = new List<ITimer>();

        public Bot(IDiscord discord, IGuildDataAccess guildData, ICalendarDataAccess calendarData)
        {
            _discord = discord;
            _discordMessages = discord as IDiscordMessages;
            _guildData = guildData;
            _calendarData = calendarData;
        }

        public async Task RunAsync()
        {
            await _discord.RunAsync();
            //InitializeTimers();
        }

        public void InitializeTimers()
        {
            _timers.Add(new CalendarTimer(new CalendarService(_discordMessages, _calendarData, new GuildService(_guildData))));
            _timers.ForEach(x => x.Start());
        }
    }
}