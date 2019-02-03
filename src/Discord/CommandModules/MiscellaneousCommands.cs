using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Discord.Convertors;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.CommandModules
{
    public class MiscellaneousCommands
    {
        private readonly IDiscordMessages _discordMessages;
        private readonly DiscordEntityConvertor _convertor;

        public MiscellaneousCommands(IDiscordMessages discordMessages, DiscordEntityConvertor convertor)
        {
            _discordMessages = discordMessages;
            _convertor = convertor;
        }

        [Command("clear")]
        [RequireOwner]
        public async Task ClearMessages(CommandContext context, int amount)
        {
            try
            {
                var count = await _discordMessages.DeleteBulkAsync(context.Channel.Id, amount, context.Message.Id);
                var embed = new BotEmbed();
                embed.Description = $"`{count}` message(s) have been removed.";
                await _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, embed);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                var embed = new BotEmbed();
                embed.Description = "An error occured while processing your command.";

                _ = _discordMessages.SendMessageAsync(context.Message.ChannelId, string.Empty, embed);
            }

        }
    }
}