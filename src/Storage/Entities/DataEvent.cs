using System;

namespace Storage.Entities
{
    public class DataEvent
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
        public bool active { get; set; }
        public string event_name { get; set; }
        public DateTime start_date { get; set; }
        public DateTime end_date { get; set; }
        public DateTimeOffset deleted_at { get; set; }
        public string discord_id { get; set; }
        public ulong guild_id { get; set; } = 512271618224029702; //todo: GET ID FROM DATABASE
    }
}