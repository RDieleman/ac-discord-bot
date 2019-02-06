using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Storage
{
    public interface IMemberDataAccess
    {
        Task<IEnumerable<ClanMember>> GetClanMembers(int clanId);
        Task<ClanMember> GetClanMember(int clanId, ulong discordId);
    }
}