using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
    public class BotGuild
    {
        public ulong GuildId { get; }
        private readonly List<Calendar> _calendars;
        private readonly List<Event> _events;

        public IEnumerable<Event> GetEvents()
        {
            return _events.AsEnumerable();
        }

        public IEnumerable<Calendar> GetCalendars()
        {
            return _calendars.AsEnumerable();
        }


        public BotGuild(ulong guildId, IEnumerable<Calendar> calendars = null, IEnumerable<Event> events = null)
        {
            GuildId = guildId;
            _calendars = calendars?.ToList() ?? new List<Calendar>();
            _events = events?.ToList() ?? new List<Event>();
        }
    }
}