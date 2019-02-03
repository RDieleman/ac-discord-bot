using System.Threading.Tasks;

namespace Core.Entities.Interactive
{
    public interface ICriterion<in T>
    {
        Task<bool> JudgeAsync(BotCommandContext sourceContext, T parameter);
    }
}