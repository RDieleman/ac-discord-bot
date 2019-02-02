using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Exceptions;
using Core.Services;
using Discord.Convertors;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord.CommandModules
{
    public class AttendanceCommands
    {
        private readonly EventService _eventService;
        private readonly DiscordEntityConvertor _convertor;
        private readonly ClanService _clanService;
        private readonly IDiscordMessages _discordMessages;

        public AttendanceCommands(EventService eventService, ClanService clanService, DiscordEntityConvertor convertor, IDiscordMessages discordMessages)
        {
            _eventService = eventService;
            _clanService = clanService;
            _convertor = convertor;
            _discordMessages = discordMessages;
        }

        [Command("event")]
        public async Task SetAttendance(CommandContext context, int eventId)
        {
            //todo: set command permissions
            try
            {             
                var clan = _clanService.GetClan(context.Channel.GuildId);

                var attendees = new List<DiscordMember>();

                //add mentioned users to list of attendees
                foreach (var messageMentionedUser in context.Message.MentionedUsers.Distinct().Where(x => !x.IsBot).ToList())
                {
                    attendees.Add(await context.Guild.GetMemberAsync(messageMentionedUser.Id));
                }

                //get list of attendees from leader's voice channel
                var leader = await context.Guild.GetMemberAsync(context.User.Id);
                var voiceChannel = leader.VoiceState?.Channel;
                if (voiceChannel != null)
                {
                    attendees.AddRange((context.Guild.Members.Where(x => x.VoiceState?.Channel?.Id == voiceChannel.Id && !x.IsBot)).Distinct());
                }


                var botMembers = new List<BotMember>();
                foreach (var discordMember in attendees)
                {
                    botMembers.Add(_convertor.DiscordMemberToBotMember(discordMember));
                }

                try
                {
                    await _eventService.SetAttendance(clan.Id, eventId, botMembers.AsEnumerable());
                }
                catch (AttendanceTrackedException)
                {
                    var errorEmbed = new BotEmbed();
                    errorEmbed.Description = $"Attendance for event `{eventId}` has already been tracked.";

                    _ = _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, errorEmbed);
                    return;
                }
                catch (EventNotFoundException)
                {
                    var errorEmbed = new BotEmbed();
                    errorEmbed.Description = $"No event found with id `{eventId}`.";

                    _ = _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, errorEmbed);
                    return;
                }

                var embed = new BotEmbed();
                embed.Description = $"Attendance for event `{eventId}` has been tracked.";

                _ = _discordMessages.SendAndDeleteMessageAsync(clan.CommandChannelId, string.Empty, embed);
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