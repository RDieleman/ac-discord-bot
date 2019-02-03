using System;

namespace Core.Entities
{
    public class BotUser
    {
        public string AvatarHash { get; }
        public string AvatarUrl { get; }
        public string DefaultAvatarUrl { get; }
        public string Discriminator { get; }
        public string Email { get; }
        public bool IsBot { get; }
        public bool IsCurrent { get; }
        public string Mention { get; }
        public bool? MfaEnabled { get; }
        public string Username { get; }
        public bool? Verified { get; }
        public ulong Id { get; }
        public DateTimeOffset CreationTimestamp { get; }

        public BotUser(string avatarHash, string avatarUrl, string defaultAvatarUrl, string discriminator, string email,
            bool isBot, bool isCurrent, string mention, bool? mfaEnabled, string username, bool? verified, ulong id,
            DateTimeOffset creationTimestamp)
        {
            AvatarHash = avatarHash;
            AvatarUrl = avatarUrl;
            DefaultAvatarUrl = defaultAvatarUrl;
            Discriminator = discriminator;
            Email = email;
            IsBot = isBot;
            IsCurrent = isCurrent;
            Mention = mention;
            MfaEnabled = mfaEnabled;
            Username = username;
            Verified = verified;
            Id = id;
            CreationTimestamp = creationTimestamp;
        }
    }
}