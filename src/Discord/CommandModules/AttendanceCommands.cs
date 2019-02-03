using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Core.Entities.Interview;
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
        private readonly InterviewService _interviewService;

        public AttendanceCommands(EventService eventService, ClanService clanService, DiscordEntityConvertor convertor, IDiscordMessages discordMessages, InterviewService interviewService)
        {
            _eventService = eventService;
            _clanService = clanService;
            _convertor = convertor;
            _discordMessages = discordMessages;
            _interviewService = interviewService;
        }

        [Command("event")]
        public async Task SetAttendance(CommandContext context)
        {
            //todo: set command permissions
            try
            {
                var clan = _clanService.GetClan(context.Channel.GuildId);
                Event @event = null;

                //get in progress events 
                var eventsInProgress = (await _eventService.GetInProgressEvents(clan.Id)).ToList();
                if (!eventsInProgress.Any())
                {
                    var errorEmbed = new BotEmbed();
                    errorEmbed.Description = $"There aren't any events in progress at the moment.";

                    _ = _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, errorEmbed);
                    return;
                }
                else if (eventsInProgress.Count() > 1)
                {
                    var options = new List<string>();
                    foreach (var eventInProgress in eventsInProgress)
                    {
                        options.Add($"{eventInProgress.Name}");
                    }

                    var questions = new List<Question>();

                    questions.Add(new MultipleChoice("Please choose the event you want to track attendance for.",
                        "Event", 60, options));
                    var botContext = _convertor.CommandContextToBotContext(context);

                    var answers = await _interviewService.ProcessInterviewAsync(botContext, clan, questions);
                    @event = eventsInProgress.Find(x => x.Name.Equals(answers[0].Content));
                }
                else
                {
                    @event = eventsInProgress.First();
                }

                var attendees = new List<DiscordMember>();

                //add mentioned users to list of attendees
                foreach (var messageMentionedUser in context.Message.MentionedUsers.Distinct().Where(x => !x.IsBot)
                    .ToList())
                {
                    attendees.Add(await context.Guild.GetMemberAsync(messageMentionedUser.Id));
                }

                //get list of attendees from leader's voice channel
                var leader = await context.Guild.GetMemberAsync(context.User.Id);
                var voiceChannel = leader.VoiceState?.Channel;
                if (voiceChannel != null)
                {
                    attendees.AddRange(
                        (context.Guild.Members.Where(x => x.VoiceState?.Channel?.Id == voiceChannel.Id && !x.IsBot))
                        .Distinct());
                }
                else
                {
                    var questions = new List<Question>();
                    questions.Add(new Polar($"You are currently not in a voice-channel which means no attendees will be tracked. {Environment.NewLine}Are you sure you want to continue?", "Confirm", 60));
                    var botContext = _convertor.CommandContextToBotContext(context);

                    var answer = (await _interviewService.ProcessInterviewAsync(botContext, clan, questions)).ToList().First().Content;
                    if(!answer.Equals("Y", StringComparison.InvariantCultureIgnoreCase)) throw new ProcessCanceledException();
                }


                var botMembers = new List<BotMember>();
                foreach (var discordMember in attendees)
                {
                    botMembers.Add(_convertor.DiscordMemberToBotMember(discordMember));
                }

                var missingAttendees = new List<BotMember>();

                try
                {
                    await _eventService.SetAttendance(clan.Id, @event, botMembers.AsEnumerable());
                }
                catch (AttendanceTrackedException ex)
                {
                    var errorEmbed = new BotEmbed();
                    errorEmbed.Description = $"Attendance for event `{ex.EventName}` has already been tracked.";

                    _ = _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, errorEmbed);
                    return;
                }
                catch (EventNotFoundException ex)
                {
                    var errorEmbed = new BotEmbed();
                    errorEmbed.Description = $"No event found with id `{@event.Id}`.";

                    _ = _discordMessages.SendAndDeleteMessageAsync(context.Channel.Id, string.Empty, errorEmbed);
                    return;
                }
                catch (AttendanceMembersMissing ex)
                {
                    missingAttendees.AddRange(ex.Members);
                }

                var embed = new BotEmbed();
                embed.Author = new BotAuthor(name: "Event attendance tracked");
                embed.Description = $"► Name: `{@event.Name}`{Environment.NewLine}" +
                                    $"► Host: `{@event.LeaderName}`{Environment.NewLine}" +
                                    $"► Attendees: `{botMembers.Count}`{Environment.NewLine}" +
                                    $"► Tracked by: {leader.Mention}";

                _ = _discordMessages.SendMessageAsync(clan.CommandChannelId, string.Empty, embed);

                if (missingAttendees.Count > 0)
                {
                    var missingEmbed = new BotEmbed();
                    missingEmbed.Author = new BotAuthor(name:"Attendees missing from the clan roster");
                    var mentions = new List<string>();
                    foreach (var missingAttendee in missingAttendees)
                    {
                        mentions.Add($"► <@{missingAttendee.Id}>");
                    }

                    missingEmbed.Description = string.Join(Environment.NewLine, mentions);

                    await _discordMessages.SendMessageAsync(clan.CommandChannelId, string.Empty, missingEmbed);
                }
            }
            catch (ProcessCanceledException)
            {
                var embed = new BotEmbed();
                embed.Description = "Your command has been cancelled.";

                _ = _discordMessages.SendAndDeleteMessageAsync(context.Message.ChannelId, string.Empty, embed);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                var embed = new BotEmbed();
                embed.Description = "An error occured while processing your command.";

                _ = _discordMessages.SendAndDeleteMessageAsync(context.Message.ChannelId, string.Empty, embed);
            }
        }
    }
}