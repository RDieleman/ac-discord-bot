using System;
using System.Threading.Tasks;
using System.Timers;
using Core.Services;
using Discord.Entities;

namespace Core.Entities.Timers
{
    public class CalendarTimer : ITimer
    {
        private Timer _loopingTimer;
        private readonly CalendarService _service;

        public CalendarTimer(CalendarService service)
        {
            _service = service;
        }

        public void Start()
        {
            var now = DateTimeService.GetDateTimeNow();

            var minutes = now.Minute;
            var seconds = now.Second;

            var waitTime = TimeSpan.Zero;
            if (minutes < 15)
                waitTime = TimeSpan.FromMinutes(15 - minutes) - TimeSpan.FromSeconds(seconds);
            else if (minutes < 30)
                waitTime = TimeSpan.FromMinutes(30 - minutes) - TimeSpan.FromSeconds(seconds);
            else if (minutes < 45)
                waitTime = TimeSpan.FromMinutes(45 - minutes) - TimeSpan.FromSeconds(seconds);
            else
                waitTime = TimeSpan.FromMinutes(60 - minutes) - TimeSpan.FromSeconds(seconds);

            _loopingTimer = new Timer
            {
                Interval = 10000, //todo: change this TimeSpan.FromMinutes(15).TotalMilliseconds,
                AutoReset = true,
                Enabled = false
            };

            _loopingTimer.Elapsed += UpdateCalendars;

            var syncTimer = new Timer
            {
                AutoReset = false,
                Interval = 10000 //todo: change this waitTime.TotalMilliseconds,
            };

            syncTimer.Elapsed += (sender, e) =>
            {
                _loopingTimer.Start(); 
                UpdateCalendars(null, null);
            };
            syncTimer.Start();
        }

        private async void UpdateCalendars(object sender, ElapsedEventArgs e)
        {
            await _service.UpdateCalendarsAsync();
        }
    }
}