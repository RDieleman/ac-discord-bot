using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface IGuildDataAccess
    {
        Task<IEnumerable<BotGuild>> GetAllGuildsAsync();

        Task AddGuildAsync(BotGuild guild);

        Task<BotGuild> GetGuildAsync(ulong guildId);
    }
}