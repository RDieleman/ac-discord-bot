using System.Threading.Tasks;
using Core.Entities;
using Core.Exceptions;
using Core.Storage;

namespace Core.Services
{
    public class MemberService
    {
        private readonly IMemberDataAccess _memberData;

        public MemberService(IMemberDataAccess memberData)
        {
            _memberData = memberData;
        }

        public async Task<ClanMember> GetClanMember(int clanId, BotMember member)
        {
            var clanMember = await _memberData.GetClanMember(clanId, member.Id);
            if(clanMember == null) throw new MemberDataNotFoundException(member);
            return clanMember;
        }
    }
}