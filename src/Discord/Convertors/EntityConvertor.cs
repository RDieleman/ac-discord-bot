using System;
using Core.Entities;
using DSharpPlus.Entities;

namespace Discord.Convertors
{
    public class EntityConvertor
    {
        public DiscordEmbed BotEmbedToDiscordEmbed(BotEmbed embed)
        {
            var builder = new DiscordEmbedBuilder();

            if (!(embed.Author is null))
                builder.WithAuthor(embed.Author.Name, embed.Author.Url, embed.Author.IconUrl);

            if (!(embed.Color is null))
                builder.WithColor(new DiscordColor(embed.Color.R, embed.Color.G, embed.Color.B));

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
            return new BotChannel(discordChannel.Id, discordChannel.GuildId);
        }
    }
}
