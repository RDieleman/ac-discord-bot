using System;

namespace Storage.Entities
{
    public class DataMember
    {
        public int id { get; set; }
        public int clan_id { get; set; }
        public int user_id { get; set; }
        public string rsn { get; set; }
        public string discord { get; set; }
        public string discord_id { get; set; }
        public string rank { get; set; }
        public string status { get; set; }
        public string rsjclean { get; set; }
        public string bank { get; set; }
        public DateTime joindate { get; set; }
        public DateTime trialend { get; set; }
        public string notes { get; set; }
        public int count_events_attended { get; set; }
        public string id_events_attended { get; set; }
        public DateTime date_events_attended { get; set; }
        public string highscores { get; set; }
        public string highscores_month { get; set; }
        public string highscores_month_old { get; set; }
        public DateTime highscores_last_updated { get; set; }
        public bool rsn_not_found { get; set; }
        public DateTimeOffset created_at { get; set; }
        public DateTimeOffset updated_at { get; set; }
        public DateTimeOffset deleted_at { get; set; }
    }
}