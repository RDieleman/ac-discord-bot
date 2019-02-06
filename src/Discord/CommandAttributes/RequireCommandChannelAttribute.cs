using System;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Exceptions;
using Core.Services;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireCommandChannelAttribute : CheckBaseAttribute
    {

        public override Task<bool> CanExecute(CommandContext ctx, bool help)
        {
            var discordMessages = ctx.CommandsNext.Dependencies.GetDependency<IDiscordMessages>();
            var clanService = ctx.CommandsNext.Dependencies.GetDependency<ClanService>();

            try
            {
                if (ctx.Guild == null || ctx.Member == null)
                    return Task.FromResult(false);

                var clan = clanService.GetClan(ctx.Channel.GuildId);
                if (clan.CommandChannelId != ctx.Channel.Id)
                    throw new CommandChannelException(clan.CommandChannelId);

                return Task.FromResult(true);
            }
            catch (CommandChannelException ex)
            {
                var embed = new BotEmbed();
                embed.Description = $"That command can only be used in <#{ex.ChannelId}>.";
                _= discordMessages.SendAndDeleteMessageAsync(ctx.Channel.Id, "", embed);
                return Task.FromResult(false);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);

                var embed = new BotEmbed();
                embed.Description = $"An error occured while processing your command.{Environment.NewLine}Please try again or contact a moderator if this keeps happening.";

                _ = discordMessages.SendAndDeleteMessageAsync(ctx.Channel.Id, string.Empty, embed);
                return Task.FromResult(false);
            }
        }
    }
}