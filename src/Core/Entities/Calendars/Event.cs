using System;

namespace Core.Entities
{
    public class Event
    {
        public int Id { get; }
        public ulong LeaderDiscordId { get; }
        public string Name { get; }
        public bool Active { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }

        public Event(int id,  ulong leaderDiscordId, string name, bool active, DateTime startDateTime, DateTime endDateTime)
        {
            Id = id;
            LeaderDiscordId = leaderDiscordId;
            Name = name;
            Active = active;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }
    }
}