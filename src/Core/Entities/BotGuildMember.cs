namespace Core.Entities
{
    public class BotGuildMember
    {
        public ulong Id { get; }
        public string Username { get; }
        public string Nickname { get; }

        public BotGuildMember(ulong id, string username, string nickname = null)
        {
            Id = id;
            Username = username;
            Nickname = nickname;
        }
    }
}