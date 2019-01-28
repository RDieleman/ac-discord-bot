using System;

namespace Core.Entities
{
    public class Event
    {
        public int Id { get; }
        public BotGuildMember Leader { get; }
        public string Name { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }

        public Event(int id, BotGuildMember leader, string name, DateTime startDateTime, DateTime endDateTime)
        {
            Id = id;
            Leader = leader;
            Name = name;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
        }
    }
}