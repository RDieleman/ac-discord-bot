using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Core.Configuration;
using Core.Entities;
using Core.Storage;
using Dapper;
using MySql.Data.MySqlClient;
using Storage.Convertors;
using Storage.Entities;

namespace Storage.DatabaseAccess
{
    public class DatabaseCalendarDataAccess : ICalendarDataAccess
    {
        private readonly IDatabaseConfig _config;
        private readonly DataEntityConvertor _convertor;

        public DatabaseCalendarDataAccess(IDatabaseConfig config, DataEntityConvertor convertor)
        {
            _config = config;
            _convertor = convertor;
        }

        public async Task<IEnumerable<Calendar>> GetCalendarsFromGuild(ulong guildId)
        {
            var sql = "SELECT * FROM calendar_settings WHERE deleted_at IS NULL;";

            var dataCalendars = new List<DataCalendar>();
            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                dataCalendars = connection.Query<DataCalendar>(sql).ToList();
            }

            var calendars = new List<Calendar>();
            foreach (var dataCalendar in dataCalendars)
            {
                calendars.Add(_convertor.DataCalendarToCalendar(dataCalendar));
            }

            return calendars.AsEnumerable();
        }

        public async Task UpdateCalendarMessageId(int calendarId, ulong discordMessageId)
        {
            var sql = "UPDATE calendar_settings SET calendar_message_id = @MessageId WHERE id = @CalendarId;";

            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                connection.Execute(sql, new {MessageId = discordMessageId, CalendarId = calendarId});
            }
        }
    }
}