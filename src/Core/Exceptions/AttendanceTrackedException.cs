using System;

namespace Core.Exceptions
{
    public class AttendanceTrackedException : Exception
    {
        public AttendanceTrackedException(string message) : base(message)
        {

        }
    }
}