namespace Core.Entities
{
    public class ClanMember
    {
        public int Id { get; }
        public int ClanId { get; }
        public string Rsn { get; }
        public ulong DiscordId { get; }

        public ClanMember(int id, int clanId, string rsn, ulong discordId)
        {
            Id = id;
            ClanId = clanId;
            Rsn = rsn;
            DiscordId = discordId;
        }
    }
}