using System;
using System.Collections.Generic;
using Core.Exceptions;

namespace Core.Entities.Interview
{
    public class MultipleChoice : Question
    {
        public List<string> Options { get; }

        public MultipleChoice(string Long, string Short, int secondsToAnswer, List<string> options) : base(Long, Short, secondsToAnswer)
        {
            Options = options;
        }

        public override string ProcessAnswer(string answer)
        {
            answer = FilterInput(answer);

            if (int.TryParse(answer, out var n))
            {
                //check if value given is an option
                if (n < 1 || n > Options.Count)
                {
                    throw new InvalidOptionException(1, Options.Count);
                }
            }
            else
            {
                throw new InvalidAnswerException();
            }

            return Options[n - 1];
        }

        public override string GetContent()
        {
            var options = new List<string>();
            var index = 1;
            foreach (var option in Options)
            {
                options.Add($"[{index}] {option}");
                index++;
            }

            return $"```ini{Environment.NewLine}{Long}{Environment.NewLine}{Environment.NewLine}{string.Join(Environment.NewLine, options)}```";
        }
    }
}