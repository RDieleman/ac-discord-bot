using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Entities
{
    public class BotEmbed
    {
        public BotAuthor Author { get; set; }
        public string ColorHex { get; set; }
        public string Description { get; set; }
        public BotFooter Footer { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public DateTime? Timestamp { get; set; }
        public Uri Url { get; set; }
        private readonly List<BotField> _fields = new List<BotField>();

        public IEnumerable<BotField> GetFields()
            => _fields.AsEnumerable();

        public void RemoveField(BotField field)
            => _fields.Remove(field);

        public void AddField(BotField field)
            => _fields.Add(field);
    }

    public class BotAuthor
    {
        public string IconUrl { get; }
        public string Name { get; }
        public string Url { get; }

        public BotAuthor(string iconUrl = null, string name = null, string url = null)
        {
            IconUrl = iconUrl;
            Name = name;
            Url = url;
        }
    }

    public class BotField
    {
        public string Name { get; }
        public string Value { get; }
        public bool Inline { get; set; }

        public BotField(string name, string value, bool inline = false)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }
    }

    public class BotFooter
    {
        public string Text { get; }
        public string IconUrl { get; }

        public BotFooter(string text = null, string iconUrl = null)
        {
            Text = text;
            IconUrl = iconUrl;
        }
    }
}