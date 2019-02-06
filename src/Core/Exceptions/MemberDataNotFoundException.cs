using System;
using Core.Entities;

namespace Core.Exceptions
{
    public class MemberDataNotFoundException : Exception
    {
        private BotMember _botMember;
        public MemberDataNotFoundException(BotMember botMember)
        {
            _botMember = botMember;
        }
    }
}