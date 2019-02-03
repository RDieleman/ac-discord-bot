using System;

namespace Core.Exceptions
{
    public class InvalidOptionException : Exception
    {
        public int MinIndex { get; }
        public int MaxIndex { get; }

        public InvalidOptionException(int minIndex, int maxIndex)
        {
            MinIndex = minIndex;
            MaxIndex = maxIndex;
        }
        public InvalidOptionException(string message, int minIndex, int maxIndex)
            : base(message)
        {
            MinIndex = minIndex;
            MaxIndex = maxIndex;
        }

        public InvalidOptionException(string message, Exception inner, int minIndex, int maxIndex)
            : base(message, inner)
        {
            MinIndex = minIndex;
            MaxIndex = maxIndex;
        }
    }
}