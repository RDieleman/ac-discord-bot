namespace Core.Entities
{
    public class BotMessage
    {
        public ulong MessageId { get; }
        public ulong ChannelId { get; }

        public BotMessage(ulong messageId, ulong channelId)
        {
            MessageId = messageId;
            ChannelId = channelId;
        }
    }
}