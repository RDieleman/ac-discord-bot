using Core.Exceptions;

namespace Core.Entities.Interview
{
    public class Regular : Question
    {
        public int MaxAnswerLength { get; }

        public Regular(string Long, string Short, int secondsToAnswer, int maxAnswerLength) : base(Long,
            Short, secondsToAnswer)
        {
            MaxAnswerLength = maxAnswerLength;
        }

        public override string ProcessAnswer(string answer)
        {
            answer = FilterInput(answer);

            //answer has to be either y or n
            if (answer.Length > MaxAnswerLength || answer.Length < 1)
            {
                throw new AnswerLengthException(MaxAnswerLength);
            }

            return answer;
        }
    }
}