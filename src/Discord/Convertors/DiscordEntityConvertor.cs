using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

namespace Discord.Convertors
{
    public class DiscordEntityConvertor
    {
        private readonly ICalendarDataAccess _calendarData;
        private readonly IEventDataAccess _eventData;

        public DiscordEntityConvertor(ICalendarDataAccess calendarData, IEventDataAccess eventData)
        {
            _calendarData = calendarData;
            _eventData = eventData;
        }

        public DiscordEmbed BotEmbedToDiscordEmbed(BotEmbed embed)
        {
            var builder = new DiscordEmbedBuilder();

            if (!(embed.Author is null))
                builder.WithAuthor(embed.Author.Name, embed.Author.Url, embed.Author.IconUrl);

            if (!(embed.ColorHex is null))
                builder.WithColor(new DiscordColor(embed.ColorHex));

            if (!(embed.Description is null))
                builder.WithDescription(embed.Description);

            if (!(embed.Footer is null))
                builder.WithFooter(embed.Footer.Text, embed.Footer.IconUrl);

            if (!(embed.ThumbnailUrl is null))
                builder.WithThumbnailUrl(embed.ThumbnailUrl);

            if ((embed.Timestamp.HasValue))
                builder.WithTimestamp(embed.Timestamp);

            if (!(embed.Title is null))
                builder.WithTitle(embed.Title);

            if (!(embed.Url is null))
                builder.WithUrl(embed.Url);

            if (!(embed.ImageUrl is null))
                builder.WithImageUrl(embed.ImageUrl);

            return builder.Build();
        }

        public BotMessage DiscordMessageToBotMessage(DiscordMessage message)
        {
            var author = DiscordUserToBotUser(message.Author);
            var channel = DiscordChannelToBotChannel(message.Channel);
            return new BotMessage(author, message.Id, channel.ChannelId, message.Content);
        }

        public BotChannel DiscordChannelToBotChannel(DiscordChannel discordChannel)
            => new BotChannel(discordChannel.GuildId, discordChannel.Id);

        public BotGuild DiscordGuildToBotGuild(DiscordGuild guild)
            => new BotGuild(guild.Id);

        public BotMember DiscordMemberToBotMember(DiscordMember member)
            => new BotMember(member.Id, member.Email, member.Username, member.Nickname, member.DisplayName, member.IsBot, member.MfaEnabled, member.Verified, member.IsOwner, member.JoinedAt, member.AvatarUrl);

        public BotCommandContext CommandContextToBotContext(CommandContext context)
        {
            var channel = DiscordChannelToBotChannel(context.Channel);
            var command = CommandToBotCommand(context.Command);
            var guild = DiscordGuildToBotGuild(context.Guild);
            var member = context.Member != null ? DiscordMemberToBotMember(context.Member) : null;
            var message = DiscordMessageToBotMessage(context.Message);
            var user = context.User != null ? DiscordUserToBotUser(context.User) : null;

            return new BotCommandContext(channel, command, guild, member, user, message, context.RawArgumentString);
        }

        public BotCommand CommandToBotCommand(Command command)
        {
            var arguments = new List<BotCommandArgument>();
            foreach (var commandArgument in command.Arguments)
            {
                arguments.Add(CommandArgumentToBotCommandArgument(commandArgument));
            }

            return new BotCommand(command.Name, command.Aliases.AsEnumerable(), arguments.AsEnumerable(), command.Description, command.QualifiedName);
        }

        public BotCommandArgument CommandArgumentToBotCommandArgument(CommandArgument argument)
            => new BotCommandArgument(argument.Name, argument.DefaultValue, argument.Description, argument.IsCatchAll, argument.IsOptional, argument.Type);

        public BotUser DiscordUserToBotUser(DiscordUser user)
            => new BotUser(user.AvatarHash, user.AvatarUrl, user.DefaultAvatarUrl, user.Discriminator, user.Email,
                user.IsBot, user.IsCurrent, user.Mention, user.MfaEnabled, user.Username, user.Verified, user.Id,
                user.CreationTimestamp);
    }
}
