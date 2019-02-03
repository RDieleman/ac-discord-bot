using System;
using System.Threading.Tasks;

namespace Core.Entities.Interactive
{
    public class EnsureSourceUserCriterion : ICriterion<BotMessage>
    {
        public Task<bool> JudgeAsync(BotCommandContext sourceContext, BotMessage parameter)
        {
            var ok = sourceContext.User.Id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }
}