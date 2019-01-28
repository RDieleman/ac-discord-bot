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
            //todo: implement question for offset

            await _service.CreateCalendarAsync(_convertor.DiscordMessageToBotMessage(context.Message), 0);
        }

        [Command("update")]
        public async Task UpdateCalendar(BotGuild guild)
        {
            await _service.UpdateGuildCalendars(guild);
        }
    }
}