using System;
using System.Threading.Tasks;
using Core.Entities;
using Core.Services;
using Discord.Convertors;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Discord.CommandModules
{
    //[Group("cal")]
    public class CalendarCommand
    {
        private readonly CalendarService _service;

        private readonly EntityConvertor _convertor;

        public CalendarCommand(CalendarService service, EntityConvertor convertor)
        {
            _service = service;
            _convertor = convertor;
        }

        [Command("new")]
        public async Task CreateCalendar(CommandContext context)
        {
            try
            {
                //todo: implement question for offset
                var guild = await _convertor.DiscordGuildToBotGuild(context.Guild);
                var channel = _convertor.DiscordChannelToBotChannel(context.Channel);
                await _service.CreateCalendarAsync(guild, channel, 0);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Command("update")]
        public async Task UpdateCalendar(CommandContext context)
        {
            try
            {
                var guild = await _convertor.DiscordGuildToBotGuild(context.Guild);
                await _service.UpdateGuildCalendars(guild);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        [Command("updateall")]
        public async Task UpdateCalendars(CommandContext context)
        {
            try
            {
                await _service.UpdateCalendarsAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}