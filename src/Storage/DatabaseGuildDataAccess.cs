using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Storage
{
    public class DatabaseGuildDataAccess : IGuildDataAccess
    {
        public Task<IEnumerable<BotGuild>> GetAllGuildsAsync()
        {
            throw new System.NotImplementedException();
        }

        public Task AddGuildAsync(BotGuild guild)
        {
            throw new System.NotImplementedException();
        }

        public Task<BotGuild> GetGuildAsync(ulong guildId)
        {
            throw new System.NotImplementedException();
        }

        public Task GetGuild(ulong guildId)
        {
            throw new System.NotImplementedException();
        }
    }
}