using System.Threading.Tasks;

namespace Core
{
    public class Bot
    {
        private readonly IDiscord _discord;

        public Bot(IDiscord discord)
        {
            _discord = discord;
        }

        public async Task RunAsync()
        {
            await _discord.RunAsync();
        }

    }
}