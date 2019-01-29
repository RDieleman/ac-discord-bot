using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface IEventDataAccess
    {
        Task<IEnumerable<Event>> GetGuildEventsAsync(ulong guildId);
        Task AddEventAsync(ulong guildId, Event @event);
    }
}