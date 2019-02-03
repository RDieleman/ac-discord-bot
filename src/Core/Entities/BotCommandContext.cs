namespace Core.Entities
{
    public class BotCommandContext
    {
        public BotChannel Channel { get; }
        public BotCommand Command { get; }
        public BotGuild Guild { get; }
        public BotMember Member { get; }
        public BotMessage Message { get; }
        public BotUser User { get; }
        public string RawArgumentString { get; }

        public BotCommandContext(BotChannel channel, BotCommand command, BotGuild guild, BotMember member, BotUser user, BotMessage message, string rawArgumentString)
        {
            Channel = channel;
            Command = command;
            Guild = guild;
            Member = member;
            Message = message;
            RawArgumentString = rawArgumentString;
            User = user;
        }
    }
}