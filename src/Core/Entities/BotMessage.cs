namespace Core.Entities
{
    public class BotMessage
    {
        public BotUser Author { get; }
        public ulong MessageId { get; }
        public ulong ChannelId { get; }
        public string Content { get; }

        public BotMessage(BotUser author, ulong messageId, ulong channelId, string content)
        {
            Author = author;
            MessageId = messageId;
            ChannelId = channelId;
            Content = content;
        }
    }
}