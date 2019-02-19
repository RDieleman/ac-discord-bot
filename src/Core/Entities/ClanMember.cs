using System;
using System.Collections.Generic;

namespace Core.Entities
{
    public class ClanMember
    {
        public int Id { get; }
        public int ClanId { get; }
        public string Rsn { get; }
        public ulong DiscordId { get; }
        public int EventCount { get; }
        public DateTime JoinDate { get; }
        public IReadOnlyList<int> IdEventsAttended { get; }

        public ClanMember(int id, int clanId, string rsn, ulong discordId, int eventCount, DateTime joinDate, IReadOnlyList<int> idEventsAttended)
        {
            Id = id;
            ClanId = clanId;
            Rsn = rsn;
            DiscordId = discordId;
            EventCount = eventCount;
            JoinDate = joinDate;
            IdEventsAttended = idEventsAttended;
        }
    }
}