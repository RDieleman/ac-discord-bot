using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Exceptions;
using Core.Services;
using Core.Storage;
using Discord.Convertors;
using DSharpPlus.CommandsNext;

namespace Discord.CommandModules
{
    public class MemberCommands
    {
        private readonly DiscordEntityConvertor _convertor;
        private readonly MemberService _memberService;
        private readonly ClanService _clanService;
        private readonly IDiscordMessages _discordMessages;

        public MemberCommands(DiscordEntityConvertor convertor, MemberService memberService, ClanService clanService, IDiscordMessages discordMessages)
        {
            _convertor = convertor;
            _memberService = memberService;
            _clanService = clanService;
            _discordMessages = discordMessages;
        }

        [Command("stats")]
        public async Task GetMemberStats(CommandContext context)
        {
            try
            {
                var clan = _clanService.GetClan(context.Guild.Id);
                var botMember = _convertor.DiscordMemberToBotMember(context.Member);
                var clanMemberData = await _memberService.GetClanMember(clan.Id, botMember);

                var statEmbed = new BotEmbed();
                statEmbed.Author = new BotAuthor(context.Member.AvatarUrl ?? context.Member.DefaultAvatarUrl,
                    context.Member.Nickname ?? context.Member.Username);
                statEmbed.Description =
                    $"► RSN: `{clanMemberData.Rsn ?? "Runescape name not found"}`{Environment.NewLine}" +
                    $"► Join date: `{clanMemberData.JoinDate.ToString("yyyy MMMM dd")}`{Environment.NewLine}" +
                    $"► Event count: `{clanMemberData.EventCount}`";

                await _discordMessages.SendMessageAsync(context.Channel.Id, string.Empty, statEmbed);
            }
            catch (GuildNotFoundException)
            {
                var errorEmbed = new BotEmbed();
                errorEmbed.Description = $"No clan data found for your discord server.";

                _ = _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, errorEmbed);
                return;
            }
            catch (MemberDataNotFoundException ex)
            {
                var errorEmbed = new BotEmbed();
                errorEmbed.Description = $"You data could not be found.{Environment.NewLine}Please contact a moderator to be added to the clan roster.";

                _ = _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, errorEmbed);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                var embed = new BotEmbed();
                embed.Description = $"An error occured while processing your command.{Environment.NewLine}Please try again or contact a moderator if this keeps happening.";

                _ = _discordMessages.SendAndDeleteMessageAsync(context.Message.ChannelId, string.Empty, embed);
            }
        }
    }
}