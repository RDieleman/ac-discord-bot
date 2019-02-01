using System;

namespace Core.Exceptions
{
    public class EventNotFoundException : Exception
    {
        public EventNotFoundException(string message) : base(message)
        {
        }
    }
}