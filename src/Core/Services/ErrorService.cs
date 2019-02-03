using System.Threading.Tasks;
using Core.Discord;
using Core.Exceptions;

namespace Core.Services
{
    public class ErrorService
    {
        private readonly IDiscordMessages _discordMessages;

        public ErrorService(IDiscordMessages discordMessages)
        {
            _discordMessages = discordMessages;
        }
    }
}