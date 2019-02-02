namespace Core.Entities
{
    public class BotGuild
    {
        public ulong Id { get; }

        public BotGuild(ulong id)
        {
            Id = id;
        }
    }
}