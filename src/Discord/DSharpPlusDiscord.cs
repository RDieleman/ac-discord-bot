using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord.Convertors;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using Core;
using Core.Configuration;
using Core.Discord;
using Core.Entities;
using Core.Entities.Interactive;
using Core.Entities.Timers;
using Core.Services;
using Core.Storage;
using Discord.CommandModules;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
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

        public DSharpPlusDiscord(IBotConfiguration botConfiguration,  DiscordEntityConvertor entityConvertor, ICalendarDataAccess calendarData, IEventDataAccess eventData, IClanDataAccess clanData)
        {
            _botConfiguration = botConfiguration;
            _entityConvertor = entityConvertor;
            _dataAccess = new DataAccess(calendarData, eventData, clanData);
        }

        public async Task RunAsync()
        {
            await InitializeDependencyCollection();

            await InitializeDiscordClientAsync();

            InitializeCommandsNextModuleAsync();

            InitializeTimers();
        }

        private async Task InitializeDependencyCollection()
        {
            using (var builder = new DependencyCollectionBuilder())
            {
                builder.AddInstance(_entityConvertor);
                builder.AddInstance(new DateTimeService());
                builder.AddInstance(new InterviewService(this));
                builder.AddInstance(new EventService(_dataAccess.EventData));
                builder.AddInstance(new ClanService(await _dataAccess.ClanData.GetClansAsync(), _dataAccess.ClanData));
                builder.AddInstance(this as IDiscordMessages);
                builder.AddInstance(this as IDiscordGuilds);
                builder.AddInstance(this as IDiscordMembers);
                _dependencyCollection = builder.Build();

                builder.AddInstance(new CalendarService(this, _dataAccess.CalendarData, _dependencyCollection.GetDependency<ClanService>(), _dependencyCollection.GetDependency<EventService>()));
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
            _commandsNextModule.RegisterCommands<CalendarCommands>();
            _commandsNextModule.RegisterCommands<AttendanceCommands>();
            _commandsNextModule.RegisterCommands<MiscellaneousCommands>();
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
                CustomPrefixPredicate = CheckPrefix,
                EnableDms = false,    
                CaseSensitive = false
            };
        }

        private Task<int> CheckPrefix(DiscordMessage msg)
        {
            if (msg.Channel.IsPrivate) return Task.FromResult(-1);
            var service = _dependencyCollection.GetDependency<ClanService>();
            var clan = service.GetClan(msg.Channel.GuildId);
            if (clan == null) return Task.FromResult(-1);
            //todo: update clans if clan not found
            if (!msg.Content.StartsWith(clan.Prefix)) return Task.FromResult(-1);

            if (msg.ChannelId != clan.CommandChannelId)
            {
                var embed = new BotEmbed();
                embed.Description = "You can't use commands in this channel.";
                _= SendAndDeleteMessageAsync(msg.ChannelId, "", embed);
                _ = DeleteMessageAsync(_entityConvertor.DiscordMessageToBotMessage(msg));
                return Task.FromResult(-1);
            }

            var commandMessage = _entityConvertor.DiscordMessageToBotMessage(msg);
            _ = DeleteMessageAsync(commandMessage, TimeSpan.FromMilliseconds(750));

            return Task.FromResult(clan.Prefix.Length);
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

            try
            {
                if (embed != null && string.IsNullOrWhiteSpace(embed.ColorHex))
                {
                    var bot = await channel.Guild.GetMemberAsync(_discordClient.CurrentUser.Id);
                    embed.ColorHex = bot.Color.ToString();
                }
            }
            catch
            {
                Console.WriteLine("Failed to get bot message color.");
            }

            DiscordEmbed discordEmbed = null;
            if (embed != null)
            {
                discordEmbed = _entityConvertor.BotEmbedToDiscordEmbed(embed);
            }

            var discordMessage = await channel.SendMessageAsync(message, false, discordEmbed);          

            return _entityConvertor.DiscordMessageToBotMessage(discordMessage);
        }

        public async Task SendAndDeleteMessageAsync(ulong channelId, string message = "", BotEmbed embed = null,
            TimeSpan? delay = null)
        {
            delay = delay ?? TimeSpan.FromSeconds(_botConfiguration.GetDeleteDelaySeconds());

            var msg = await SendMessageAsync(channelId, message, embed);
            _= DeleteMessageAsync(msg, delay);
        }

        public async Task EditMessageAsync(ulong channelId, ulong messageId, string message = null, BotEmbed embed = null)
        {
            var channel = await _discordClient.GetChannelAsync(channelId);

            var discordMessage = await channel.GetMessageAsync(messageId);

            try
            {
                if (embed != null && string.IsNullOrWhiteSpace(embed.ColorHex))
                {
                    var bot = await channel.Guild.GetMemberAsync(_discordClient.CurrentUser.Id);
                    var role = bot.Roles.Last();
                    embed.ColorHex = role.Color.ToString();
                }
            }
            catch
            {
                Console.WriteLine("Failed to get bot message color.");
            }

            DiscordEmbed discordEmbed = null;
            if (embed != null)
                discordEmbed = _entityConvertor.BotEmbedToDiscordEmbed(embed);


            await discordMessage.ModifyAsync(message, discordEmbed);
        }

        public async Task<int> DeleteBulkAsync(ulong channelId, int amount, ulong commandId)
        {
            if (amount > 100) amount = 100;
            var channel = await _discordClient.GetChannelAsync(channelId);
            var messages = (await channel.GetMessagesAsync(amount, before: commandId)).ToList();
            await channel.DeleteMessagesAsync(messages);
            return messages.Count;
        }

        public async Task DeleteMessageAsync(BotMessage targetMessage, TimeSpan? delay = null)
        {
            var channel = await _discordClient.GetChannelAsync(targetMessage.ChannelId);
            var discordMessage = await channel.GetMessageAsync(targetMessage.MessageId);

            if (delay.HasValue) await Task.Delay(delay.Value);
            await discordMessage.DeleteAsync();
        }

        public Task<BotMessage> NextMessageAsync(BotCommandContext context,
            bool fromSourceUser = true,
            bool inSourceChannel = true,
            TimeSpan? timeout = null, CancellationToken token = default(CancellationToken))
        {
            var criterion = new Criteria<BotMessage>();
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureSourceUserCriterion());
            if (inSourceChannel)
                criterion.AddCriterion(new EnsureSourceChannelCriterion());
            return NextMessageAsync(context, criterion, timeout, token);
            
        }

        public async Task<BotMessage> NextMessageAsync(BotCommandContext context, ICriterion<BotMessage> criterion,
            TimeSpan? timeout = null, CancellationToken token = default(CancellationToken))
        {
            timeout = timeout ?? TimeSpan.FromSeconds(15);

            var eventTrigger = new TaskCompletionSource<DiscordMessage>();
            var cancelTrigger = new TaskCompletionSource<bool>();

            token.Register(() => cancelTrigger.SetResult(true));

            async Task _discordClient_MessageCreated(MessageCreateEventArgs e)
            {
                var botMessage = _entityConvertor.DiscordMessageToBotMessage(e.Message);
                var result = await criterion.JudgeAsync(context, botMessage).ConfigureAwait(false);
                if (result)
                    eventTrigger.SetResult(e.Message);
            }

            _discordClient.MessageCreated += _discordClient_MessageCreated;

            var trigger = eventTrigger.Task;
            var cancel = cancelTrigger.Task;
            var delay = Task.Delay(timeout.Value);
            var task = await Task.WhenAny(trigger, delay, cancel).ConfigureAwait(false);

            _discordClient.MessageCreated -= _discordClient_MessageCreated;

            if (task == trigger)
            {
                var msg = await trigger.ConfigureAwait(false);
                return _entityConvertor.DiscordMessageToBotMessage(msg);
            }
            else
                return null;
        }

        private Task _discordClient_MessageCreated(MessageCreateEventArgs e)
        {
            throw new NotImplementedException();
        }

        public async Task LeaveGuildAsync(ulong guildId)
        {
            var guild = await _discordClient.GetGuildAsync(guildId);
            await guild.LeaveAsync();
        }

        public Task<IEnumerable<BotGuild>> GetGuildsAsync()
        {
            var botGuilds = new List<BotGuild>();
            foreach (var discordGuild in _discordClient.Guilds)
            {
                botGuilds.Add(_entityConvertor.DiscordGuildToBotGuild(discordGuild.Value));
            }

            return Task.FromResult(botGuilds.AsEnumerable());
        }

        public async Task<BotGuild> GetGuildAsync(ulong guildId)
        {
            var discordGuild = await _discordClient.GetGuildAsync(guildId);
            return _entityConvertor.DiscordGuildToBotGuild(discordGuild);
        }

        public async Task<BotMember> GetBotGuildMember(ulong guildId, ulong memberId)
        {
            var guild = await _discordClient.GetGuildAsync(guildId);
            var member = await guild.GetMemberAsync(memberId);
            return _entityConvertor.DiscordMemberToBotMember(member);
        }
    }
}