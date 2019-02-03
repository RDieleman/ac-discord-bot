using System.Threading.Tasks;

namespace Core.Entities.Interactive
{
    public class EnsureSourceChannelCriterion : ICriterion<BotMessage>
    {
        public Task<bool> JudgeAsync(BotCommandContext sourceContext, BotMessage parameter)
        {
            var ok = sourceContext.Channel.ChannelId == parameter.ChannelId;
            return Task.FromResult(ok);
        }
    }
}