namespace WorkingCalendar.Application;

public interface ICalendarRepository
{
    Task<string> GetCalendarXmlAsync(int year, string culture);
}
