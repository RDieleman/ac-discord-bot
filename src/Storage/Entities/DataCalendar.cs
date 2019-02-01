using System;

namespace Storage.Entities
{
    public class DataCalendar
    {
        public int id { get; set; }
        public string calendar_name { get; set; }
        public string calendar_channel_id { get; set; }
        public string calendar_message_id { get; set; }
        public string calendar_color { get; set; }
        public string calendar_utc_offset { get; set; }
        public DateTimeOffset? created_at { get; set; }
        public DateTimeOffset? updated_at { get; set; }
        public DateTimeOffset? deleted_at { get; set; }
    }
}