using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Discord.Configuration;
using Discord.Convertors;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Core;
using Core.Discord;
using Core.Entities;
using Core.Services;
using Core.Storage;
using Discord.CommandModules;
using Discord.Entities;
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
        private readonly IGuildDataAccess _guildData;
        private readonly ICalendarDataAccess _calendarData;

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
        }

        private void InitializeDependencyCollection()
        {
            using (var builder = new DependencyCollectionBuilder())
            {
                builder.AddInstance(_entityConvertor);
                builder.AddInstance(new DateTimeService());
                builder.AddInstance(new GuildService(_guildData));
                builder.AddInstance(new CalendarService(this, _calendarData, builder.GetDependency<GuildService>()));
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

            //register commands
            _commandsNextModule.RegisterCommands<CalendarCommand>();
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

        public async Task<BotMessage> SendMessageAsync(BotChannel targetChannel, string message = "", BotEmbed embed = null)
        {
            var guild = await _discordClient.GetGuildAsync(targetChannel.GuildId);
            var channel = guild.GetChannel(targetChannel.ChannelId);

            DiscordEmbed discordEmbed = null;
            if (embed != null)
            {
                discordEmbed = _entityConvertor.BotEmbedToDiscordEmbed(embed);
            }

            var discordMessage = await channel.SendMessageAsync(message, false, discordEmbed);
            return _entityConvertor.DiscordMessageToBotMessage(discordMessage);
        }

        public async Task EditMessageAsync(BotMessage targetMessage, string message = null, BotEmbed embed = null)
        {
            var channel = await _discordClient.GetChannelAsync(targetMessage.ChannelId);
            var discordMessage = await channel.GetMessageAsync(targetMessage.MessageId);

            DiscordEmbed discordEmbed = null;
            if (embed != null)
                discordEmbed = _entityConvertor.BotEmbedToDiscordEmbed(embed);

            await discordMessage.ModifyAsync(message, discordEmbed);
        }

        public async Task DeleteMessageAsync(BotMessage targetMessage)
        {
            var channel = await _discordClient.GetChannelAsync(targetMessage.ChannelId);
            var discordMessage = await channel.GetMessageAsync(targetMessage.MessageId);

            await discordMessage.DeleteAsync();
        }
    }
}