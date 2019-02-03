using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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

        public async Task SetAttendance(int clanId, Event @event, IEnumerable<BotMember> attendees)
        {
            if(@event.AttendanceDone) throw  new AttendanceTrackedException(@event.Name);

            var attendeesStringIds = new List<string>();
            foreach (var botMember in attendees)
            {
                attendeesStringIds.Add($"{Convert.ToString(botMember.Id)}"); //todo: fix this once ids get fixed in the database
            }

            await _eventData.TrackAttendance(clanId, @event.Id, attendeesStringIds);
        }

        public async Task<IEnumerable<Event>> GetClanEvents(int clanId)
        {
            return await _eventData.GetGuildEventsAsync(clanId);
        }

        public async Task<IEnumerable<Event>> GetInProgressEvents(int clanId)
        {
            var now = DateTime.UtcNow;
            var events = await GetClanEvents(clanId);
            return events.Where(x =>
                x.Active && 
                !x.AttendanceDone &&
                now.CompareTo(x.EndDateTime.ToUniversalTime()) <= 0 &&
                now.CompareTo(x.StartDateTime.ToUniversalTime()) >= 0);
        }
    }
}