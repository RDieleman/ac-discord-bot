using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Entities.Interactive
{
    public class Criteria<T> : ICriterion<T>
    {
        private List<ICriterion<T>> _criteria = new List<ICriterion<T>>();

        public Criteria<T> AddCriterion(ICriterion<T> criterion)
        {
            _criteria.Add(criterion);
            return this;
        }

        public async Task<bool> JudgeAsync(BotCommandContext sourceContext, T parameter)
        {
            foreach (var criterion in _criteria)
            {
                var result = await criterion.JudgeAsync(sourceContext, parameter).ConfigureAwait(false);
                if (!result) return false;
            }

            return true;
        }
    }
}