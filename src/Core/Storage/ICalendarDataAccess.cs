using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface ICalendarDataAccess
    {
        Task<IEnumerable<Calendar>> GetAllCalendars();

        Task<IEnumerable<Calendar>> GetCalendarsFromGuild(ulong guildId);

        Task AddCalendar(Calendar calendar);

        Task DeleteCalendar(int calendarId);
    }
}