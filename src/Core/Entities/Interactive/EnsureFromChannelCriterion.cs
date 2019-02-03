using System.Threading.Tasks;

namespace Core.Entities.Interactive
{
    public class EnsureFromChannelCriterion : ICriterion<BotMessage>
    {
        private readonly ulong _channelId;

        public EnsureFromChannelCriterion(BotChannel channel)
            => _channelId = channel.ChannelId;

        public Task<bool> JudgeAsync(BotCommandContext sourceContext, BotMessage parameter)
        {
            bool ok = _channelId == parameter.ChannelId;
            return Task.FromResult(ok);
        }
    }
}