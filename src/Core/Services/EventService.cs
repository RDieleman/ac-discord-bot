using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Entities;
using Core.Storage;

namespace Core.Services
{
    public class EventService
    {
        private readonly IEventDataAccess _eventData;

        public EventService(IEventDataAccess eventData)
        {
            _eventData = eventData;
        }

        public async Task SetAttendance(int eventId, IEnumerable<BotMember> attendees)
        {
            var attendeesStringIds = new List<string>();
            foreach (var botMember in attendees)
            {
                attendeesStringIds.Add($"{Convert.ToString(botMember.Id)}"); //todo: fix this once ids get fixed in the database
            }

            await _eventData.TrackAttendance(eventId, attendeesStringIds);
        }
    }
}