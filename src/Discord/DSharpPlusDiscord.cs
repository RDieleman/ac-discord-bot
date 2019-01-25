using System.Drawing;
using System.Threading.Tasks;
using Discord.Configuration;
using Discord.Convertors;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Core;
using Core.Discord;
using Core.Entities;
using DSharpPlus.Entities;

namespace Discord
{
    public class DSharpPlusDiscord : IDiscord, IDiscordMessages
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

        public async Task<BotMessage> SendMessage(BotChannel targetChannel, string message = null, BotEmbed embed = null)
        {
            var guild = await _discordClient.GetGuildAsync(targetChannel.GuildId);
            var channel = guild.GetChannel(targetChannel.ChannelId);

            DiscordEmbed discordEmbed = null;
            if (embed != null)
                discordEmbed = _entityConvertor.BotEmbedToDiscordEmbed(embed);

            var discordMessage = await channel.SendMessageAsync(null, false, discordEmbed);
            return _entityConvertor.DiscordMessageToBotMessage(discordMessage);
        }

        public async Task EditMessage(BotMessage targetMessage, string message = null, BotEmbed embed = null)
        {
            var guild = await _discordClient.GetGuildAsync(targetMessage.Channel.GuildId);
            var channel = guild.GetChannel(targetMessage.Channel.ChannelId);
            var discordMessage = await channel.GetMessageAsync(targetMessage.MessageId);

            DiscordEmbed discordEmbed = null;
            if (embed != null)
                discordEmbed = _entityConvertor.BotEmbedToDiscordEmbed(embed);

            await discordMessage.ModifyAsync(message, discordEmbed);
        }

        public async Task DeleteMessage(BotMessage targetMessage)
        {
            var guild = await _discordClient.GetGuildAsync(targetMessage.Channel.GuildId);
            var channel = guild.GetChannel(targetMessage.Channel.ChannelId);
            var discordMessage = await channel.GetMessageAsync(targetMessage.MessageId);

            await discordMessage.DeleteAsync();
        }
    }
}