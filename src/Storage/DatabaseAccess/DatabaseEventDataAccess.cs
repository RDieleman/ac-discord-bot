using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Discord;
using Core.Entities;
using Core.Storage;
using Dapper;
using MySql.Data.MySqlClient;
using Storage.Convertors;
using Storage.Entities;

namespace Storage
{
    public class DatabaseEventDataAccess : IEventDataAccess
    {
        private readonly IDatabaseConfig _config;
        private readonly DataEntityConvertor _convertor;

        public DatabaseEventDataAccess(IDatabaseConfig config, DataEntityConvertor convertor)
        {
            _config = config;
            _convertor = convertor;
        }

        public async Task<IEnumerable<Event>> GetGuildEventsAsync(int clanId)
        {
            //todo: fix async task issue

            var dataEvents = new List<DataEvent>();

            var now = DateTime.UtcNow;
            var dateTimeMinString = now.Subtract(TimeSpan.FromHours(12)).ToString("yyyy-MM-dd 00:00:00");
            var dateTimeMaxString = now.Add(TimeSpan.FromHours(14)).Add(TimeSpan.FromDays(7)).ToString("yyyy-MM-dd 23:59:59");

            var sql ="SELECT e.*, u.name FROM events e INNER JOIN users u ON e.user_id = u.id WHERE e.clan_id = @ClanId AND ((e.start_date > @minDate AND e.start_date < @maxDate) || (e.end_date > @minDate AND e.end_date < @maxDate))";
     
            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                dataEvents = connection.Query<DataEvent>(sql, new { ClanId = clanId, minDate = dateTimeMinString, maxDate = dateTimeMaxString }).ToList();
            }

            var events = new List<Event>();
            foreach (var dataEvent in dataEvents)
            {
                events.Add(_convertor.DataEventToEvent(dataEvent));
            }

            return events.AsEnumerable();
        }

        public async Task TrackAttendance(int clanId, int eventId, IEnumerable<string> attendeesIds)
        {

            var sql =
                "UPDATE members SET id_events_attended=concat_ws(',', id_events_attended, @EventId), count_events_attended = count_events_attended + 1 WHERE clan_id = @ClanId AND discord_id IN @Ids;" +
                "UPDATE events SET event_tracked = 1 WHERE clan_id = @ClanId AND id = @EventId";

            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                connection.Execute(sql, new { ClanId = clanId, EventId = eventId,  Ids = attendeesIds.ToArray()});
            }
        }

        public async Task<Event> GetEvent(int clanId, int eventId)
        {
            var sql = "SELECT e.*, u.name FROM events e INNER JOIN users u ON e.user_id = u.id WHERE e.clan_id = @ClanId AND e.id = @EventId;";
            IEnumerable<DataEvent> dataEvents = null;
            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                dataEvents = connection.Query<DataEvent>(sql, new {ClanId = clanId, EventId = eventId});
            }
            var events = new List<Event>();
            foreach (var dataEvent in dataEvents)
            {
                events.Add(_convertor.DataEventToEvent(dataEvent));
            }

            return events.FirstOrDefault();
        }
    }
}