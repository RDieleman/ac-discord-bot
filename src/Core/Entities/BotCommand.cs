using System.Collections.Generic;

namespace Core.Entities
{
    public class BotCommand
    {
        public string Name { get; }
        public IEnumerable<string> Aliases { get; }
        public IEnumerable<BotCommandArgument> Arguments { get; }
        public string Description { get; }
        public string QualifiedName { get; }

        public BotCommand(string name, IEnumerable<string> aliases, IEnumerable<BotCommandArgument> arguments, string description, string qualifiedName)
        {
            Name = name;
            Aliases = aliases;
            Arguments = arguments;
            Description = description;
            QualifiedName = qualifiedName;
        }
    }
}