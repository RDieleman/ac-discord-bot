using System;

namespace Core.Exceptions
{
    public class AnswerLengthException : Exception
    {
        public int Length { get; }

        public AnswerLengthException(int max)
        {
            Length = max;
        }
        public AnswerLengthException(string message, int max)
            : base(message)
        {
            Length = max;
        }

        public AnswerLengthException(string message, Exception inner, int max)
            : base(message, inner)
        {
            Length = max;
        }
    }
}