using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface IEventDataAccess
    {
        Task<IEnumerable<Event>> GetGuildEventsAsync(int clanId);
        Task<int> TrackAttendance(int clanId, int eventId, IEnumerable<string> attendeesStringIds);
        Task<Event> GetEvent(int clanId, int eventId);
    }
}