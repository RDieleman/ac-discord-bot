using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Discord
{
    public interface IDiscordGuilds
    {
        Task LeaveGuildAsync(ulong guildId);
        Task<IEnumerable<BotGuild>> GetGuildsAsync();
        Task<BotGuild> GetGuildAsync(ulong guildId);
    }
}