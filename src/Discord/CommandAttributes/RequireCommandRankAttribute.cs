using System;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Exceptions;
using Core.Services;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord.CommandAttributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireCommandRankAttribute : CheckBaseAttribute
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

                var commandRole = ctx.Guild.GetRole(clan.CommandRoleId);

                var allowed = false;

                foreach (var discordRole in ctx.Member.Roles)
                {
                    if (discordRole.Position >= commandRole.Position)
                    {
                        allowed = true;
                        break;
                    }
                }

                if (!allowed)
                    throw new CommandRankException();

                return Task.FromResult(true);
            }
            catch (CommandRankException)
            {
                var embed = new BotEmbed();
                embed.Description = "You don't have the required permissions to use this command.";
                _ = discordMessages.SendAndDeleteMessageAsync(ctx.Channel.Id, "", embed);
                return Task.FromResult(false);
            }
            catch (Exception ex)
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