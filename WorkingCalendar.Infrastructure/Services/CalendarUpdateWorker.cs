using Cronos;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkingCalendar.Infrastructure.Services
{
    public class CalendarUpdateWorker : BackgroundService
    {
        private readonly ICalendarDataUpdater _updater;
        private readonly ILogger<CalendarUpdateWorker> _logger;
        private readonly CronExpression _cron;

        public CalendarUpdateWorker(ICalendarDataUpdater updater, IOptions<CalendarUpdateOptions> options, ILogger<CalendarUpdateWorker> logger)
        {
            _updater = updater;
            _logger = logger;
            _cron = CronExpression.Parse(options.Value.Schedule);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _updater.UpdateDataAsync(stoppingToken);
                var next = _cron.GetNextOccurrence(DateTime.UtcNow);
                if (next.HasValue)
                {
                    var delay = next.Value - DateTime.UtcNow;
                    if (delay < TimeSpan.Zero)
                    {
                        delay = TimeSpan.Zero;
                    }
                    _logger.LogInformation("Next calendar update scheduled in {Delay}", delay);
                    await Task.Delay(delay, stoppingToken);
                }
                else
                {
                    await Task.Delay(TimeSpan.FromDays(30), stoppingToken);
                }
            }
        }
    }
}
