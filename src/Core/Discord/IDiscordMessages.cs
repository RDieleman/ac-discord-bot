using System;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.Interactive;

namespace Core.Discord
{
    public interface IDiscordMessages
    {
        Task<BotMessage> SendMessageAsync(ulong channelId, string message = null, BotEmbed embed = null);

        Task SendAndDeleteMessageAsync(ulong channelId, string message = "", BotEmbed embed = null, TimeSpan? delay = null);

        Task DeleteMessageAsync(BotMessage targetMessage, TimeSpan? delay = null);

        Task EditMessageAsync(ulong channelId, ulong messageId, string message = null, BotEmbed embed = null);

        Task<int> DeleteBulkAsync(ulong channelId, int amount, ulong commandId);

        Task<BotMessage> NextMessageAsync(BotCommandContext context, ICriterion<BotMessage> criterion,
            TimeSpan? timeout = null, CancellationToken token = default(CancellationToken));

        Task<BotMessage> NextMessageAsync(BotCommandContext context, bool fromSourceUser = true,bool inSourceChannel = true,
            TimeSpan? timeout = null, CancellationToken token = default(CancellationToken));
    }
}