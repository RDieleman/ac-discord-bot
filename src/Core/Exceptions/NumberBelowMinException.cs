using System;

namespace Core.Exceptions
{
    public class NumberBelowMinException : Exception
    {
        public int MinValue { get; }

        public NumberBelowMinException(int min)
        {
            MinValue = min;
        }

        public NumberBelowMinException(string message, int min)
            : base(message)
        {
            MinValue = min;
        }

        public NumberBelowMinException(string message, Exception inner, int min)
            : base(message, inner)
        {
            MinValue = min;
        }
    }
}