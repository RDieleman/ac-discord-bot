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
            => new Event(dataEvent.id, dataEvent.name, dataEvent.event_name, dataEvent.active, dataEvent.start_date, dataEvent.end_date, dataEvent.allday, dataEvent.event_tracked);

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

        public Clan DataClanToClan(DataClan clan)
        {
            var serverId = StringDiscordIdToUlongDiscordId(clan.discord_server_id);
            var commandChannelId = StringDiscordIdToUlongDiscordId(clan.discord_command_channel_id);
            var commandRankId = StringDiscordIdToUlongDiscordId(clan.discord_command_rank_id);

            //todo: add notification channel id once added to the db
            return new Clan(clan.id, serverId, clan.prefix, commandChannelId, commandChannelId, commandRankId);
        }

        public ClanMember DataMemberToClanMember(DataMember member)
        {
            var discordId = StringDiscordIdToUlongDiscordId(member.discord_id);
            return new ClanMember(member.id, member.clan_id, member.rsn, discordId);
        }

    }
}