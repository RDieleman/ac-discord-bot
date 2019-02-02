using System;

namespace Storage.Entities
{
    public class DataClan
    {
        public int id { get; set; }
        public string name { get; set; }
        public string subtitle { get; set; }
        public bool active { get; set; }
        public string image_url { get; set; }
        public string prefix { get; set; }
        public string discord_server_id { get; set; }
        public string discord_command_channel_id { get; set; }
        public string discord_command_rank_id { get; set; }
        public string discord_bot_link_1 { get; set; }
        public string discord_bot_link_2 { get; set; }
        public string discord_bot_link_3 { get; set; }
        public string discord_bot_link_4 { get; set; }
        public string discord_bot_link_name_1 { get; set; }
        public string discord_bot_link_name_2 { get; set; }
        public string discord_bot_link_name_3 { get; set; }
        public string discord_bot_link_name_4 { get; set; }
        public string contact_detail_1 { get; set; }
        public string contact_detail_2 { get; set; }
        public string contact_detail_3 { get; set; }
        public int member_trial_period { get; set; }
        public string member_creation_text_1 { get; set; }
        public string member_creation_text_2 { get; set; }
        public string event_creation_text_1 { get; set; }
        public string event_creation_text_2 { get; set; }
        public string calendar_creation_text_1 { get; set; }
        public string calendar_creation_text_2 { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
        public DateTimeOffset deleted_at { get; set; }
    }
}