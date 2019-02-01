namespace Core.Storage
{
    public class DataAccess
    {
        public ICalendarDataAccess CalendarData { get; }
        public IEventDataAccess EventData { get; }

        public DataAccess(ICalendarDataAccess calendarData, IEventDataAccess eventData)
        {
            CalendarData = calendarData;
            EventData = eventData;
        }
    }
}