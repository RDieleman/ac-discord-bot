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
    public class DatabaseClanDataAccess : IClanDataAccess
    {
        private readonly IDatabaseConfig _config;
        private readonly DataEntityConvertor _convertor;

        public DatabaseClanDataAccess(IDatabaseConfig config, DataEntityConvertor convertor)
        {
            _config = config;
            _convertor = convertor;
        }

        public Task<Clan> GetClanAsync(ulong discordId)
        {
            //todo: DISCORD SERVER ID HAS TO BE UNIQUE IN DATABASE NEED WAY TO VERIFY OWNERSHIP WHEN ADDED
            var sql = "SELECT * FROM calendar_settings WHERE deleted_at IS NULL AND discord_server_id = @DiscordId LIMIT 1;";

            var dataClans = new List<DataClan>();
            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                dataClans = connection.Query<DataClan>(sql).ToList();
            }

            var clans = new List<Clan>();
            foreach (var dataClan in dataClans)
            {
                clans.Add(_convertor.DataClanToClan(dataClan));
            }

            return Task.FromResult(clans.FirstOrDefault());
        }

        public Task<IEnumerable<Clan>> GetClansAsync()
        {
            var sql = "SELECT * FROM clan_settings WHERE deleted_at IS NULL;";

            var dataClans = new List<DataClan>();
            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                dataClans = connection.Query<DataClan>(sql).ToList();
            }

            var clans = new List<Clan>();
            foreach (var dataClan in dataClans)
            {
                clans.Add(_convertor.DataClanToClan(dataClan));
            }

            return Task.FromResult(clans.AsEnumerable());
        }
    }
}