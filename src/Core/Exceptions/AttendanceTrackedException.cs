using System;

namespace Core.Exceptions
{
    public class AttendanceTrackedException : Exception
    {
        public string EventName { get; }

        public AttendanceTrackedException(string name)
        {
            EventName = name;
        }
    }
}