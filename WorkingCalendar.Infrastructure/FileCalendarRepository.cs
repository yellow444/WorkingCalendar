using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using WorkingCalendar.Application;

namespace WorkingCalendar.Infrastructure;

public class CalendarRepositoryOptions
{
    public string BasePath { get; set; } = "Data";
    public string Culture { get; set; } = "ru";
}

public class FileCalendarRepository : ICalendarRepository
{
    private readonly CalendarRepositoryOptions _options;
    private readonly IHostEnvironment _environment;
    private readonly Dictionary<(int Year,string Culture), string> _cache = new();

    public FileCalendarRepository(IOptions<CalendarRepositoryOptions> options, IHostEnvironment environment)
    {
        _options = options.Value;
        _environment = environment;
    }

    public async Task<string> GetCalendarXmlAsync(int year, string culture)
    {
        var key = (year, culture);
        if (_cache.TryGetValue(key, out var xml))
        {
            return xml;
        }
        var basePath = Path.IsPathRooted(_options.BasePath)
            ? _options.BasePath
            : Path.Combine(_environment.ContentRootPath, _options.BasePath);
        var filePath = Path.Combine(basePath, culture, year.ToString(), "calendar.xml");
        xml = await File.ReadAllTextAsync(filePath);
        _cache[key] = xml;
        return xml;
    }

    public async Task<Dictionary<int, string>> GetAllCalendarsXmlAsync(string culture)
    {
        var result = new Dictionary<int, string>();
        var basePath = Path.IsPathRooted(_options.BasePath)
            ? _options.BasePath
            : Path.Combine(_environment.ContentRootPath, _options.BasePath);
        var culturePath = Path.Combine(basePath, culture);
        if (!Directory.Exists(culturePath))
        {
            return result;
        }
        foreach (var dir in Directory.GetDirectories(culturePath))
        {
            var name = Path.GetFileName(dir);
            if (int.TryParse(name, out var year))
            {
                var xml = await GetCalendarXmlAsync(year, culture);
                result[year] = xml;
            }
        }
        return result;
    }
}
