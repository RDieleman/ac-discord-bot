using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Discord.Convertors;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Core;
using Core.Configuration;
using Core.Discord;
using Core.Entities;
using Core.Entities.Timers;
using Core.Services;
using Core.Storage;
using Discord.CommandModules;
using DSharpPlus.Entities;
using Storage;

namespace Discord
{
    public class DSharpPlusDiscord : IDiscord, IDiscordMessages, IDiscordGuilds, IDiscordMembers
    {
        private DiscordClient _discordClient;
        private CommandsNextModule _commandsNextModule;
        private DependencyCollection _dependencyCollection;

        private readonly DiscordEntityConvertor _entityConvertor;
        private readonly DataAccess _dataAccess;
        private readonly IBotConfiguration _botConfiguration;

        private readonly List<ITimer> _timers = new List<ITimer>();

        public DSharpPlusDiscord(IBotConfiguration botConfiguration,  DiscordEntityConvertor entityConvertor, ICalendarDataAccess calendarData, IEventDataAccess eventData)
        {
            _botConfiguration = botConfiguration;
            _entityConvertor = entityConvertor;
            _dataAccess = new DataAccess(calendarData, eventData);
        }

        public async Task RunAsync()
        {
            InitializeDependencyCollection();

            await InitializeDiscordClientAsync();

            InitializeCommandsNextModuleAsync();

            InitializeTimers();
        }

        private void InitializeDependencyCollection()
        {
            using (var builder = new DependencyCollectionBuilder())
            {
                builder.AddInstance(_entityConvertor);
                builder.AddInstance(new DateTimeService());
                builder.AddInstance(new CalendarService(this, _dataAccess.CalendarData, this, this));
                builder.AddInstance(new EventService(_dataAccess.EventData));
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
            _commandsNextModule.RegisterCommands<AttendanceCommand>();
        }

        public void InitializeTimers()
        {
            _timers.Add(new CalendarTimer(_dependencyCollection.GetDependency<CalendarService>()));
            _timers.ForEach(x => x.Start());
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

        public async Task<BotMessage> SendMessageAsync(ulong channelId, string message = "", BotEmbed embed = null)
        {
            var channel = await _discordClient.GetChannelAsync(channelId);

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

        public async Task<BotMember> GetBotGuildMember(ulong guildId, ulong memberId)
        {
            var guild = await _discordClient.GetGuildAsync(guildId);
            var member = await guild.GetMemberAsync(memberId);
            return _entityConvertor.DiscordMemberToBotMember(member);
        }
    }
}