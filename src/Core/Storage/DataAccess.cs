namespace Core.Storage
{
    public class DataAccess
    {
        public ICalendarDataAccess CalendarData { get; }
        public IEventDataAccess EventData { get; }
        public IClanDataAccess ClanData { get; }
        public IMemberDataAccess MemberData { get; }

        public DataAccess(ICalendarDataAccess calendarData, IEventDataAccess eventData, IClanDataAccess clanData, IMemberDataAccess memberData)
        {
            CalendarData = calendarData;
            EventData = eventData;
            ClanData = clanData;
            MemberData = memberData;
        }
    }
}