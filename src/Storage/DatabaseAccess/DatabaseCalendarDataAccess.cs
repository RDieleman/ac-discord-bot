using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Storage
{
    public class DatabaseCalendarDataAccess : ICalendarDataAccess
    {
        public Task<IEnumerable<Calendar>> GetCalendarsFromGuild(ulong guildId)
        {
            throw new System.NotImplementedException();
        }

        public Task AddCalendar(ulong guildId, Calendar calendar)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteCalendar(ulong guildId, Calendar calendar)
        {
            throw new System.NotImplementedException();
        }
    }
}