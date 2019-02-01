using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Storage.JsonAccess
{
    public class JsonCalendarDataAccess : ICalendarDataAccess
    {
        private readonly JsonDataStorage _jsonData;

        public JsonCalendarDataAccess()
        {
            _jsonData = new JsonDataStorage();
        }

        public async Task<IEnumerable<Calendar>> GetCalendarsFromGuild(ulong guildId)
            => _jsonData.RestoreCollection<Calendar>(GetCalendarCollectionByGuildId(guildId));

        public Task UpdateCalendarMessageId(int calendarId, ulong discordMessageId)
        {
            throw new NotImplementedException();
        }

        public async Task AddCalendar(ulong guildId, Calendar calendar)
            => _jsonData.StoreObject(calendar, GetCalendarCollectionByGuildId(guildId), GetKeyByCalendar(calendar));

        public async Task DeleteCalendar(ulong guildId, Calendar calendar)
            => _jsonData.DeleteObject(GetCalendarCollectionByGuildId(guildId), GetKeyByCalendar(calendar));

        private static string GetCalendarCollectionByGuildId(ulong guildId)
            => string.Concat(guildId.ToString(), "/", "Calendars");

        private static string GetKeyByCalendar(Calendar calendar)
            => calendar.MessageId.ToString();
    }
}