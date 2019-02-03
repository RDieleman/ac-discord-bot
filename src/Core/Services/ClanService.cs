using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Core.Services
{
    public class ClanService
    {
        private readonly IClanDataAccess _clanData;
        private IEnumerable<Clan> _clans;
        private readonly IMemberDataAccess _memberData;

        public ClanService(IEnumerable<Clan> clans, IClanDataAccess clanData, IMemberDataAccess memberData)
        {
            _clans = clans;
            _clanData = clanData;
            _memberData = memberData;
        }

        public Clan GetClan(ulong discordId)
        {
            return _clans.FirstOrDefault(x => x.GuildId == discordId);
        }

        public Clan GetClan(int clanId)
        {
            return _clans.FirstOrDefault(x => x.Id == clanId);
        }

        public IEnumerable<Clan> GetClans()
        {
            return _clans;
        }

        public async Task UpdateClans()
        {
            _clans = await _clanData.GetClansAsync();        
        }

        public async Task<IEnumerable<ClanMember>> GetMembers(int clanId)
        {
            return await _memberData.GetClanMembers(clanId);
        }
    }
}