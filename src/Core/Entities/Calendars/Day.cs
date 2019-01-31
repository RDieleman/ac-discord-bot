using System;
using System.Collections.Generic;

namespace Core.Entities.Calendars
{
    public class Day
    {
        public DateTime Date { get; }
        public IEnumerable<KeyValuePair<Event, BotMember>> Events { get; }

        public Day(DateTime date, IEnumerable<KeyValuePair<Event, BotMember>> events)
        {
            Date = date;
            Events = events;
        }
    }
}