using System.Threading.Tasks;
using Core.Entities;

namespace Core.Discord
{
    public interface IDiscordMessages
    {
        Task<BotMessage> SendMessageAsync(ulong channelId, string message = null, BotEmbed embed = null);
        Task EditMessageAsync(BotMessage targetMessage, string message = null, BotEmbed embed = null);
        Task DeleteMessageAsync(BotMessage targetMessage);
    }
}