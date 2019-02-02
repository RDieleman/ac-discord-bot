namespace Core.Storage
{
    public class DataAccess
    {
        public ICalendarDataAccess CalendarData { get; }
        public IEventDataAccess EventData { get; }
        public IClanDataAccess ClanData { get; }

        public DataAccess(ICalendarDataAccess calendarData, IEventDataAccess eventData, IClanDataAccess clanData)
        {
            CalendarData = calendarData;
            EventData = eventData;
            ClanData = clanData;
        }
    }
}