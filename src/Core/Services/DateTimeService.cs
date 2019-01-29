using System;

namespace Core.Services
{
    public class DateTimeService
    {
        public static DateTime GetDateTimeNow()
        {
            return DateTime.UtcNow;
        }
    }
}