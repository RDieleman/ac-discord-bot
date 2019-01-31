using System.Threading.Tasks;
using Core.Entities;

namespace Core.Discord
{
    public interface IDiscordMembers
    {
        Task<BotMember> GetBotGuildMember(ulong guildId, ulong memberId);
    }
}