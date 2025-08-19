using WorkingCalendar.Domain;

namespace WorkingCalendar.Application;

public interface ICalendarService
{
    Task<string> CheckDayAsync(DateTime day, int workingDaysPerWeek);
    Task<string> GetYearSqlAsync(int year, string type, int workingDaysPerWeek);
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

    public async Task<string> GetYearSqlAsync(int year, string type, int workingDaysPerWeek)
    {
        var xml = await _repository.GetCalendarXmlAsync(year, "ru");
        var calendar = Calendar.FromXml(xml);
        return calendar.GenerateSql(year, type, workingDaysPerWeek);
    }
}
