using System;
using System.Collections.Generic;

namespace Core.Entities.Calendars
{
    public class Day
    {
        public DateTime Date { get; }
        public IEnumerable<Event> Events { get; }

        public Day(DateTime date, IEnumerable<Event> events)
        {
            Date = date;
            Events = events;
        }
    }
}