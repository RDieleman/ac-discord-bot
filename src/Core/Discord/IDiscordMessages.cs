using System;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Discord
{
    public interface IDiscordMessages
    {
        Task<BotMessage> SendMessageAsync(ulong channelId, string message = null, BotEmbed embed = null);
        Task SendAndDeleteMessageAsync(ulong channelId, string message = "", BotEmbed embed = null, TimeSpan? delay = null);
        Task DeleteMessageAsync(BotMessage targetMessage, TimeSpan? delay = null);
        Task EditMessageAsync(BotMessage targetMessage, string message = null, BotEmbed embed = null);
        Task<int> DeleteBulkAsync(ulong channelId, int amount, ulong commandId);
    }
}