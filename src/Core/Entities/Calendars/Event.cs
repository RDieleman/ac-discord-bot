using System;

namespace Core.Entities
{
    public class Event
    {
        public int Id { get; }
        public string LeaderName { get; }
        public string Name { get; }
        public bool Active { get; }
        public DateTime StartDateTime { get; }
        public DateTime EndDateTime { get; }
        public bool Allday { get; }
        public bool AttendanceDone { get; }

        public Event(int id,  string leaderName, string name, bool active, DateTime startDateTime, DateTime endDateTime, bool allday, bool attendanceDone)
        {
            Id = id;
            LeaderName = leaderName;
            Name = name;
            Active = active;
            StartDateTime = startDateTime;
            EndDateTime = endDateTime;
            Allday = allday;
            AttendanceDone = attendanceDone;
        }
    }
}