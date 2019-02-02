using System;
using System.Drawing;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;
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
            var channel = DiscordChannelToBotChannel(message.Channel);
            return new BotMessage(message.Id, channel.ChannelId);
        }

        public BotChannel DiscordChannelToBotChannel(DiscordChannel discordChannel)
        {
            return new BotChannel(discordChannel.GuildId, discordChannel.Id);
        }

        public async Task<BotGuild> DiscordGuildToBotGuild(DiscordGuild guild)
        {
            return new BotGuild(guild.Id);
        }

        public BotMember DiscordMemberToBotMember(DiscordMember member)
            => new BotMember(member.Id, member.Email, member.Username, member.Nickname, member.DisplayName, member.IsBot, member.MfaEnabled, member.Verified, member.IsOwner, member.JoinedAt, member.AvatarUrl);

    }
}
