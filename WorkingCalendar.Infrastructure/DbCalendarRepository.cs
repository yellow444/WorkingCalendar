using Microsoft.EntityFrameworkCore;
using WorkingCalendar.Application;
using System.Linq;

namespace WorkingCalendar.Infrastructure;

public class DbCalendarRepository : ICalendarRepository
{
    private readonly CalendarDbContext _context;

    public DbCalendarRepository(CalendarDbContext context)
    {
        _context = context;
    }

    public async Task<string> GetCalendarXmlAsync(int year, string culture)
    {
        var entry = await _context.CalendarEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Year == year && e.Culture == culture);
        if (entry == null)
        {
            throw new InvalidOperationException($"Calendar not found for {year} {culture}");
        }
        return entry.Xml;
    }

    public async Task<Dictionary<int, string>> GetAllCalendarsXmlAsync(string culture)
    {
        return await _context.CalendarEntries
            .AsNoTracking()
            .Where(e => e.Culture == culture)
            .ToDictionaryAsync(e => e.Year, e => e.Xml);
    }
}
