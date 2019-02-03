using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface ICalendarDataAccess
    {
        Task<IEnumerable<Calendar>> GetCalendarsFromGuild(int clanId);
        Task UpdateCalendarMessageId(int clanId, int calendarId, ulong discordMessageId);
    }
}