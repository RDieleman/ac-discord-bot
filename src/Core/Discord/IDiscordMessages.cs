using System.Threading.Tasks;
using Core.Entities;

namespace Core.Discord
{
    public interface IDiscordMessages
    {
        Task<BotMessage> SendMessage(BotChannel targetChannel, string message = null, BotEmbed embed = null);
        Task EditMessage(BotMessage targetMessage, string message = null, BotEmbed embed = null);
        Task DeleteMessage(BotMessage targetMessage);
    }
}