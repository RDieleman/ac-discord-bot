using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Core.Services
{
    public class GuildService
    {
        private readonly IGuildDataAccess _guildData;

        public GuildService(IGuildDataAccess guildData)
        {
            _guildData = guildData;
        }

        public async Task<IEnumerable<BotGuild>> GetAllGuildsAsync()
        {
            return await _guildData.GetAllGuildsAsync();
        }

        public async Task<BotGuild> GetGuildAsync(ulong guildId)
        {
            return await _guildData.GetGuildAsync(guildId);
        }
    }
}