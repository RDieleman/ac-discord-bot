using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
    public class Calendar
    {
        public BotMessage Message { get; }
        public int UtcOffset { get; }

        public Calendar(BotMessage message, int utcOffset = 0)
        {
            Message = message;
            UtcOffset = utcOffset;
        }
    }
}