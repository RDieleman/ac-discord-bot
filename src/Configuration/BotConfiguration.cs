using Core.Configuration;

namespace Configuration
{
    public class BotConfiguration : IBotConfiguration
    {
        private readonly IConfiguration _config;

        private const string DiscordBotTokenKey = "DiscordToken";
        private const string DeleteDelaySeconds = "DeleteDelaySeconds";

        public BotConfiguration(IConfiguration config)
        {
            _config = config;
        }

        public string GetBotToken()
            => _config.GetValueFor(DiscordBotTokenKey);

        public int GetDeleteDelaySeconds()
        {
            var delay = _config.GetValueFor(DeleteDelaySeconds);
            if (string.IsNullOrWhiteSpace(delay)) return 0;

            int.TryParse(delay, out var delayNum);
            return delayNum;
        }
    }
}