using System;

namespace WorkingCalendar.Infrastructure.Services
{
    public class CalendarUpdateOptions
    {
        public string DownloadUrl { get; set; } = "https://api.github.com/repos/xmlcalendar/data/releases/latest";
        public string[] Countries { get; set; } = Array.Empty<string>();
        public string Schedule { get; set; } = "0 0 1 * *"; // Cron expression: monthly
    }
}
