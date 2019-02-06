using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core;
using Core.Discord;
using Core.Entities;
using Core.Services;
using Core.Storage;
using Discord.CommandAttributes;
using Discord.Convertors;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.CommandModules
{
    //[Group("cal")]
    public class CalendarCommands
    {
        private readonly CalendarService _calendarService;
        private readonly DiscordEntityConvertor _convertor;
        private readonly ClanService _clanService;
        private readonly EventService _eventService;
        private readonly IDiscordMessages _discordMessages;

        public CalendarCommands(CalendarService calendarService, DiscordEntityConvertor convertor, ClanService clanService, EventService eventService, IDiscordMessages discordMessages)
        {
            _calendarService = calendarService;
            _convertor = convertor;
            _clanService = clanService;
            _discordMessages = discordMessages;
            _eventService = eventService;
        }

        [Command("update")]
        [RequireCommandRank]
        public async Task UpdateCalendar(CommandContext context)
        {
            try
            {
                var clan = _clanService.GetClan(context.Channel.GuildId);
                await _calendarService.UpdateClanCalendars(clan.Id);
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