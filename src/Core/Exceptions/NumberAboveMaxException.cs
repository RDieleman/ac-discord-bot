using System;

namespace Core.Exceptions
{
    public class NumberAboveMaxException : Exception
    {
        public int MaxValue { get; }

        public NumberAboveMaxException(int max)
        {
            MaxValue = max;
        }
        public NumberAboveMaxException(string message, int max)
            : base(message)
        {
            MaxValue = max;
        }

        public NumberAboveMaxException(string message, Exception inner, int max)
            : base(message, inner)
        {
            MaxValue = max;
        }
    }
}