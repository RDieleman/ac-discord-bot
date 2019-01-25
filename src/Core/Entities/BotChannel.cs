namespace Core.Entities
{
    public class BotChannel
    {
        public ulong GuildId { get; }
        public ulong ChannelId { get; }

        public BotChannel(ulong guildId, ulong channelId)
        {
            GuildId = guildId;
            ChannelId = channelId;
        }
    }
}