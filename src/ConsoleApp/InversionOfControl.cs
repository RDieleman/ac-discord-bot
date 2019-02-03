using Configuration;
using Core;
using Core.Configuration;
using Core.Discord;
using Core.Storage;
using Discord;
using Lamar;
using Storage;
using Storage.DatabaseAccess;
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
                c.ForSingletonOf<IDiscordMembers>().UseIfNone<DSharpPlusDiscord>();
                c.ForSingletonOf<IBotConfiguration>().UseIfNone<BotConfiguration>();
                c.ForSingletonOf<IDatabaseConfig>().UseIfNone<DatabaseConfiguration>();
                c.ForSingletonOf<IConfiguration>().UseIfNone<ConfigManager>();
                c.ForSingletonOf<ICalendarDataAccess>().UseIfNone<DatabaseCalendarDataAccess>();
                c.ForSingletonOf<IEventDataAccess>().UseIfNone<DatabaseEventDataAccess>();
                c.ForSingletonOf<IClanDataAccess>().UseIfNone<DatabaseClanDataAccess>();
                c.ForSingletonOf<IMemberDataAccess>().UseIfNone<DatabaseMemberDataAccess>();
            });
        }

    }
}