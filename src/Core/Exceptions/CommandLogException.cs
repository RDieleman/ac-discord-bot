using System;

namespace Core.Exceptions
{
    public class CommandLogException : Exception
    {
        public TimeSpan RemainingDelay;

        public CommandLogException(TimeSpan remainingDelay)
        {
            this.RemainingDelay = remainingDelay;
        }
    }
}