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
    public class DatabaseMemberDataAccess : IMemberDataAccess
    {
        private readonly IDatabaseConfig _config;
        private readonly DataEntityConvertor _convertor;

        public DatabaseMemberDataAccess(IDatabaseConfig config, DataEntityConvertor convertor)
        {
            _config = config;
            _convertor = convertor;
        }

        public Task<IEnumerable<ClanMember>> GetClanMembers(int clanId)
        {
            var sql = "SELECT * FROM members WHERE clan_id = @ClanId AND deleted_at IS NULL;";

            var dataMembers = new List<DataMember>();
            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                dataMembers = connection.Query<DataMember>(sql, new { ClanId = clanId }).ToList();
            }

            var members = new List<ClanMember>();
            foreach (var dataMember in dataMembers)
            {
                members.Add(_convertor.DataMemberToClanMember(dataMember));
            }

            return Task.FromResult(members.AsEnumerable());
        }

        public Task<ClanMember> GetClanMember(int clanId, ulong discordId)
        {
            var sql = "SELECT * FROM members WHERE clan_id = @ClanId AND discord_id = @DiscId;";

            var dataMembers = new List<DataMember>();
            using (IDbConnection connection = new MySqlConnection(_config.GetConnectionString()))
            {
                dataMembers = connection.Query<DataMember>(sql, new { ClanId = clanId, DiscId = discordId.ToString() }).ToList();
            }

            var members = new List<ClanMember>();
            foreach (var dataMember in dataMembers)
            {
                members.Add(_convertor.DataMemberToClanMember(dataMember));
            }

            return Task.FromResult(members.FirstOrDefault());
        }
    }
}