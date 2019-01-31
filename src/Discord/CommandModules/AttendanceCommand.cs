using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Services;
using Discord.Convertors;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Discord.CommandModules
{
    public class AttendanceCommand
    {
        private readonly EventService _eventService;
        private readonly DiscordEntityConvertor _convertor;

        public AttendanceCommand(EventService eventService, DiscordEntityConvertor convertor)
        {
            _eventService = eventService;
            _convertor = convertor;
        }

        [Command("event")]
        public async Task SetAttendance(CommandContext context, int eventId)
        {
            //todo: set command permissions
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
                await _eventService.SetAttendance(eventId, botMembers.AsEnumerable());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}