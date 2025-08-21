using System.Text;
using System.Linq;
using WorkingCalendar.Domain;

namespace WorkingCalendar.Application;

public interface ICalendarService
{
    Task<string> CheckDayAsync(DateTime day, int workingDaysPerWeek);
    Task<string> GetYearSqlAsync(string year, string type, int workingDaysPerWeek);
}

public class CalendarService : ICalendarService
{
    private readonly ICalendarRepository _repository;

    public CalendarService(ICalendarRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> CheckDayAsync(DateTime day, int workingDaysPerWeek)
    {
        var xml = await _repository.GetCalendarXmlAsync(day.Year, "ru");
        var calendar = Calendar.FromXml(xml);
        return calendar.DescribeDay(day, workingDaysPerWeek);
    }

    public async Task<string> GetYearSqlAsync(string year, string type, int workingDaysPerWeek)
    {
        if (year == "all")
        {
            var calendars = await _repository.GetAllCalendarsXmlAsync("ru");
            var builder = new StringBuilder();
            foreach (var entry in calendars.OrderBy(c => c.Key))
            {
                var yearCalendar = Calendar.FromXml(entry.Value);
                builder.AppendLine(yearCalendar.GenerateSql(entry.Key, type, workingDaysPerWeek));
            }
            return builder.ToString();
        }

        if (!int.TryParse(year, out var y))
        {
            throw new ArgumentException("Invalid year", nameof(year));
        }

        var xml = await _repository.GetCalendarXmlAsync(y, "ru");
        var calendar = Calendar.FromXml(xml);
        return calendar.GenerateSql(y, type, workingDaysPerWeek);
    }
}
