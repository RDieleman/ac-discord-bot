using System;

namespace Core.Exceptions
{
    public class InvalidAnswerException : Exception
    {
        public InvalidAnswerException()
        {
        }
        public InvalidAnswerException(string message)
            : base(message)
        {
        }

        public InvalidAnswerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}