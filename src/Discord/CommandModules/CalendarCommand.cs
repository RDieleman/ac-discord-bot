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

        private readonly DiscordEntityConvertor _convertor;

        public CalendarCommand(CalendarService service, DiscordEntityConvertor convertor)
        {
            _service = service;
            _convertor = convertor;
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
    }
}