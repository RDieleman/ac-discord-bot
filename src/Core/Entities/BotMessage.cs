namespace Core.Entities
{
    public class BotMessage
    {
        public ulong MessageId { get; }
        public BotChannel Channel { get; }

        public BotMessage(ulong messageId, BotChannel channel)
        {
            MessageId = messageId;
            Channel = channel;
        }
    }
}