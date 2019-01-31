using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
    public class Calendar
    {
        public int Id { get; }
        public string Name { get; }
        public string ColorHex { get; }
        public ulong ChannelId { get; }
        public ulong MessageId { get; }
        public int UtcOffset { get; }

        public Calendar(int id, string name, string colorHex, ulong channelId, ulong messageId, int utcOffset)
        {
            Id = id;
            Name = name;
            ColorHex = colorHex;
            ChannelId = channelId;
            MessageId = messageId;

            //utc min = -12 and utc max = 14
            if (utcOffset < -12) UtcOffset = -12;
            else if (utcOffset > 14) UtcOffset = 14;
            else UtcOffset = utcOffset;
        }
    }
}