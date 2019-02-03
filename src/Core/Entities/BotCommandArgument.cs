using System;

namespace Core.Entities
{
    public class BotCommandArgument
    {
        public string Name { get; }
        public Object DefaultValue { get; }
        public string Description { get; }
        public bool IsCatchAll { get; }
        public bool IsOptional { get; }
        public Type Type { get; }

        public BotCommandArgument(string name, object defaultValue, string description, bool isCatchAll, bool isOptional, Type type)
        {
            Name = name;
            DefaultValue = defaultValue;
            Description = description;
            IsCatchAll = isCatchAll;
            IsOptional = isOptional;
            Type = type;
        }
    }
}