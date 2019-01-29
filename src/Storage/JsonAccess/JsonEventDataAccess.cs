using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Storage.JsonAccess
{
    public class JsonEventDataAccess : IEventDataAccess
    {
        private readonly JsonDataStorage _jsonData;

        public JsonEventDataAccess()
        {
            _jsonData = new JsonDataStorage();
        }

        public async Task<IEnumerable<Event>> GetGuildEventsAsync(ulong guildId)
            => _jsonData.RestoreObject<IEnumerable<Event>>(GetEventCollectionByGuildId(guildId), GetKey());

        public async Task AddEventAsync(ulong guildId, Event @event)
        {
            var events = (await GetGuildEventsAsync(guildId))?.ToList() ?? new List<Event>();
            events.Add(@event);
            _jsonData.StoreObject(events.AsEnumerable(), GetEventCollectionByGuildId(guildId), GetKey());
        }

        private static string GetEventCollectionByGuildId(ulong guildId)
            => string.Concat(guildId.ToString(), "/", "Events");

        private static string GetKey()
            => "Events";
    }
}