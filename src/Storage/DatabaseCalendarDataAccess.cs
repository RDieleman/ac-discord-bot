using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Storage
{
    public class DatabaseCalendarDataAccess : ICalendarDataAccess
    {
        public Task<IEnumerable<Calendar>> GetAllCalendars()
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Calendar>> GetCalendarsFromGuild(ulong guildId)
        {
            throw new System.NotImplementedException();
        }

        public Task AddCalendar(Calendar calendar)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCalendar(int calendarId)
        {
            throw new System.NotImplementedException();
        }
    }
}