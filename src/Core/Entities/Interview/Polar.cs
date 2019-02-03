using System;
using Core.Exceptions;

namespace Core.Entities.Interview
{
    public class Polar : Question
    {
        public Polar(string Long, string Short, int secondsToAnswer) : base(Long, Short, secondsToAnswer)
        {
        }

        public override string ProcessAnswer(string answer)
        {
            answer = FilterInput(answer);

            //answer has to be either y or n
            if (!answer.Equals("Y", StringComparison.InvariantCultureIgnoreCase) &&
                !answer.Equals("N", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new PolarAnswerException();
            }

            return answer;
        }

        public override string GetContent()
        {
            return $"```ini{Environment.NewLine}[Y/N]{Environment.NewLine}{Environment.NewLine}{Long}```";
        }
    }
}