using System;
using System.Collections.Generic;
using Core.Entities;

namespace Core.Exceptions
{
    public class AttendanceMembersMissing : Exception
    {
        public IEnumerable<BotMember> Members { get; }

        public AttendanceMembersMissing(IEnumerable<BotMember> members)
        {
            Members = members;
        }
    }
}