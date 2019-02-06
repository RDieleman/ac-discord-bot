using System;

namespace Core.Exceptions
{
    public class CommandChannelException : Exception
    {
        public ulong ChannelId { get; }

        public CommandChannelException(ulong channelId)
        {
            ChannelId = channelId;
        }
    }
}