namespace WorkingCalendar.Infrastructure;

public class CalendarEntry
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string Culture { get; set; } = string.Empty;
    public string Xml { get; set; } = string.Empty;
}
