using System.Text.RegularExpressions;
using Core.Exceptions;

namespace Core.Entities.Interview
{
    public abstract class Question
    {
        public string Long { get; }
        public string Short { get; }
        public int SecondsToAnswer { get; }

        protected Question(string Long, string Short, int secondsToAnswer)
        {
            this.Long = Long;
            this.Short = Short;
            this.SecondsToAnswer = secondsToAnswer;
        }

        public virtual string ProcessAnswer(string answer)
        {
            answer = FilterInput(answer);

            if (answer.Length < 1) throw new EmptyAnswerException();

            return answer;
        }

        public virtual string GetContent()
        {
            return Long;
        }

        internal string FilterInput(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9_\-\.\/?!,&():@% ]", " ");
        }

        public override string ToString()
        {
            return Short;
        }
    }
}