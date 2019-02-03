using Core.Exceptions;

namespace Core.Entities.Interview
{
    public class Number : Question
    {
        public int AnswerValueMax;
        public int AnswerValueMin;

        public Number(string Long, string Short, int secondsToAnswer, int answerValueMax, int answerValueMin) : base(Long, Short, secondsToAnswer)
        {
            AnswerValueMax = answerValueMax;
            AnswerValueMin = answerValueMin;
        }

        public override string ProcessAnswer(string answer)
        {
            answer = FilterInput(answer);

            if (int.TryParse(answer, out var n))
            {
                if (n < AnswerValueMin)
                {
                    throw new NumberBelowMinException(AnswerValueMin);
                }
                else if (n > AnswerValueMax)
                {
                    throw new NumberAboveMaxException(AnswerValueMax);
                }
            }
            else
            {
                throw new NumberParseException();
            }

            return answer;
        }
    }
}