using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Core.Discord;
using Core.Entities;
using Storage.Entities;

namespace Storage.Convertors
{
    public class DataEntityConvertor
    {
        public Event DataEventToEvent(DataEvent dataEvent)
            => new Event(dataEvent.id, StringDiscordIdToUlongDiscordId(dataEvent.discord_id), dataEvent.event_name, dataEvent.active, dataEvent.start_date, dataEvent.end_date);

        public Calendar DataCalendarToCalendar(DataCalendar calendar)
        {            
            var messageId = StringDiscordIdToUlongDiscordId(calendar.calendar_message_id);
            var channelId = StringDiscordIdToUlongDiscordId(calendar.calendar_channel_id);
            int.TryParse(calendar.calendar_utc_offset, out var utcOffset);

            return new Calendar(calendar.id, calendar.calendar_name,  calendar.calendar_color, channelId, messageId, utcOffset);
        }

        private ulong StringDiscordIdToUlongDiscordId(string discordIdString)
        {
            //discord ids will be 0 if they are null
            if (discordIdString == null) return 0;
            discordIdString = Regex.Replace(discordIdString, @"[^0-9]", "");
            ulong.TryParse(discordIdString, out var discordId);

            return discordId;
        }

    }
}