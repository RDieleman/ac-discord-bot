using System.Threading.Tasks;

namespace Core.Entities.Interactive
{
    public class EnsureFromUserCriterion : ICriterion<BotMessage>
    {
        private readonly ulong _id;

        public EnsureFromUserCriterion(BotUser user)
            => _id = user.Id;

        public Task<bool> JudgeAsync(BotCommandContext sourceContext, BotMessage parameter)
        {
            bool ok = _id == parameter.Author.Id;
            return Task.FromResult(ok);
        }
    }
}