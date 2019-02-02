using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Core.Entities;
using Core.Exceptions;
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

        public async Task SetAttendance(int clanId, int eventId, IEnumerable<BotMember> attendees)
        {
            var @event = await _eventData.GetEvent(clanId, eventId);
            if(@event is null) throw new EventNotFoundException($"No event found with the id `{eventId}`.");
            if(@event.AttendanceDone) throw  new AttendanceTrackedException($"The attendance for the event `{@event.Name}` has already been tracked.");

            var attendeesStringIds = new List<string>();
            foreach (var botMember in attendees)
            {
                attendeesStringIds.Add($"{Convert.ToString(botMember.Id)}"); //todo: fix this once ids get fixed in the database
            }

            await _eventData.TrackAttendance(clanId, eventId, attendeesStringIds);
        }

        public async Task<IEnumerable<Event>> GetClanEvents(int clanId)
        {
            return await _eventData.GetGuildEventsAsync(clanId);
        }
    }
}