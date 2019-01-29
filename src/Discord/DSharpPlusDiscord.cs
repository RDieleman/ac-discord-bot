using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class DSharpPlusDiscord : IDiscord, IDiscordMessages, IDiscordGuilds
    {
        private DiscordClient _discordClient;
        private CommandsNextModule _commandsNextModule;
        private DependencyCollection _dependencyCollection;

        private readonly EntityConvertor _entityConvertor;
        private readonly IBotConfiguration _botConfiguration;
        private readonly ICalendarDataAccess _calendarData;
        private readonly IEventDataAccess _eventData;

        public DSharpPlusDiscord(IBotConfiguration botConfiguration, IEventDataAccess eventData, ICalendarDataAccess calendarData, EntityConvertor entityConvertor)
        {
            _botConfiguration = botConfiguration;
            _entityConvertor = entityConvertor;
            _eventData = eventData;
            _calendarData = calendarData;
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
                builder.AddInstance(new CalendarService(this, _calendarData, this));
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
            //var guild = _discordClient.Guilds.FirstOrDefault(x => x.Key == targetChannel.GuildId).Value;
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

        public async Task LeaveGuildAsync(ulong guildId)
        {
            var guild = await _discordClient.GetGuildAsync(guildId);
            await guild.LeaveAsync();
        }

        public async Task<IEnumerable<BotGuild>> GetGuildsAsync()
        {
            var botGuilds = new List<BotGuild>();
            foreach (var discordGuild in _discordClient.Guilds)
            {
                botGuilds.Add(await _entityConvertor.DiscordGuildToBotGuild(discordGuild.Value));
            }

            return botGuilds.AsEnumerable();
        }

        public async Task<BotGuild> GetGuildAsync(ulong guildId)
        {
            var discordGuild = await _discordClient.GetGuildAsync(guildId);
            return await _entityConvertor.DiscordGuildToBotGuild(discordGuild);
        }
    }
}