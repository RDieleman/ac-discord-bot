using System;

namespace Core.Entities
{
    public class BotMember
    {
        public ulong Id { get; }
        public string Email { get; }
        public string Username { get; }
        public string Nickname { get; }
        public string DisplayName { get; }
        public bool IsBot { get; }
        public bool? MfaEnabled { get; }
        public bool? IsVerified { get; }
        public bool IsOwner { get; }
        public DateTimeOffset JoinedAt { get; }
        public string AvatarUrl { get; }

        public BotMember(ulong id, string email, string username, string nickname, string displayName, bool isBot, bool? mfaEnabled, bool? isVerified, bool isOwner, DateTimeOffset joinedAt, string avatarUrl)
        {
            Id = id;
            Email = email;
            Username = username;
            Nickname = nickname;
            DisplayName = displayName;
            IsBot = isBot;
            MfaEnabled = mfaEnabled;
            IsVerified = isVerified;
            IsOwner = isOwner;
            JoinedAt = joinedAt;
            AvatarUrl = avatarUrl;
        }
    }
}