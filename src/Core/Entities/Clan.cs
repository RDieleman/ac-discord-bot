using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
    public class Clan
    {
        public int Id;
        public ulong GuildId { get; }
        public string Prefix { get; }
        public ulong NotificationChannelId { get; }
        public ulong CommandChannelId { get; }


        public Clan(int id, ulong guildId, string prefix, ulong notificationChannelId, ulong commandChannelId)
        {
            Id = id;
            GuildId = guildId;
            Prefix = !string.IsNullOrWhiteSpace(prefix) ? prefix : "!";
            NotificationChannelId = notificationChannelId; //todo: add field to database
            CommandChannelId = commandChannelId;
        }
    }
}