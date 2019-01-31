using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface ICalendarDataAccess
    {
        Task<IEnumerable<Calendar>> GetCalendarsFromGuild(ulong guildId);
        Task UpdateCalendarMessageId(int calendarId, ulong discordMessageId);
    }
}