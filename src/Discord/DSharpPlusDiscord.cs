using System.Threading.Tasks;
using Discord.Configuration;
using Discord.Convertors;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Core;

namespace Discord
{
    public class DSharpPlusDiscord : IDiscord
    {
        private DiscordClient _discordClient;
        private CommandsNextModule _commandsNextModule;
        private DependencyCollection _dependencyCollection;
        private readonly IBotConfiguration _botConfiguration;
        private readonly EntityConvertor _entityConvertor;

        public DSharpPlusDiscord(IBotConfiguration botConfiguration, EntityConvertor entityConvertor)
        {
            _botConfiguration = botConfiguration;
            _entityConvertor = entityConvertor;
        }

        public async Task RunAsync()
        {
            InitializeDependencyCollection();

            await InitializeDiscordClientAsync();

            InitializeCommandsNextModuleAsync();

            await Task.Delay(-1);
        }

        private void InitializeDependencyCollection()
        {
            using (var builder = new DependencyCollectionBuilder())
            {
                builder.AddInstance(_entityConvertor);
                _dependencyCollection = builder.Build();
            }
        }

        private async Task InitializeDiscordClientAsync()
        {
            var discordConfiguration = GetDefaultDiscordConfiguration();
            _discordClient = new DiscordClient(discordConfiguration);
            await _discordClient.ConnectAsync();
        }

        private void InitializeCommandsNextModuleAsync()
        {
            var config = GetDefaultCommandsNextConfiguration();
            _commandsNextModule = _discordClient.UseCommandsNext(config);
            //register commands here
        }

        private CommandsNextConfiguration GetDefaultCommandsNextConfiguration()
        {
            return new CommandsNextConfiguration
            {
                EnableMentionPrefix = true,
                Dependencies = _dependencyCollection,
            };
        }

        private DiscordConfiguration GetDefaultDiscordConfiguration()
        {
            return new DiscordConfiguration
            {
                Token = _botConfiguration.GetBotToken(),
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };
        }
    }
}