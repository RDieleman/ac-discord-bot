using Configuration;
using Core;
using Core.Discord;
using Core.Storage;
using Discord;
using Discord.Configuration;
using Lamar;
using Storage;
using Storage.JsonAccess;

namespace ConsoleApp
{
    public class InversionOfControl
    {
        private static Container _container;

        public static Container Container => GetOrInitContainer();

        private static Container GetOrInitContainer()
        {
            if (_container is null)
            {
                InitializeContainer();
            }

            return _container;
        }

        private static void InitializeContainer()
        {
            _container = new Container(c =>
            {
                c.ForSingletonOf<IDiscord>().UseIfNone<DSharpPlusDiscord>();
                c.ForSingletonOf<IDiscordMessages>().UseIfNone<DSharpPlusDiscord>();
                c.ForSingletonOf<IDiscordGuilds>().UseIfNone<DSharpPlusDiscord>();
                c.ForSingletonOf<IBotConfiguration>().UseIfNone<BotConfiguration>();
                c.ForSingletonOf<IConfiguration>().UseIfNone<ConfigManager>();
                c.ForSingletonOf<ICalendarDataAccess>().UseIfNone<JsonCalendarDataAccess>();
                c.ForSingletonOf<IEventDataAccess>().UseIfNone<JsonEventDataAccess>();
            });
        }

    }
}