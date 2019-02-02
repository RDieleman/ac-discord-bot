using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface IClanDataAccess
    {
        Task<Clan> GetClanAsync(ulong discordId);
        Task<IEnumerable<Clan>> GetClansAsync();
    }
}