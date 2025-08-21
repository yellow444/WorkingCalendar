using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WorkingCalendar.Infrastructure;

namespace WorkingCalendar.Server;

public class DatabaseInitializer
{
    private readonly CalendarDbContext _context;
    private readonly CalendarRepositoryOptions _options;
    private readonly IHostEnvironment _environment;

    public DatabaseInitializer(
        CalendarDbContext context,
        IOptions<CalendarRepositoryOptions> options,
        IHostEnvironment environment)
    {
        _context = context;
        _options = options.Value;
        _environment = environment;
    }

    public async Task InitializeAsync()
    {
        for (var i = 0; i < 30; i++)
        {
            try
            {
                await _context.Database.MigrateAsync();
                break;
            }
            catch (Npgsql.NpgsqlException)
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
            }
        }

        if (await _context.CalendarEntries.AnyAsync())
        {
            return;
        }

        var repository = new FileCalendarRepository(Options.Create(_options), _environment);
        var basePath = Path.IsPathRooted(_options.BasePath)
            ? _options.BasePath
            : Path.Combine(_environment.ContentRootPath, _options.BasePath);

        if (!Directory.Exists(basePath))
        {
            return;
        }

        foreach (var cultureDir in Directory.EnumerateDirectories(basePath))
        {
            var culture = Path.GetFileName(cultureDir);
            foreach (var yearDir in Directory.EnumerateDirectories(cultureDir))
            {
                if (!int.TryParse(Path.GetFileName(yearDir), out var year))
                {
                    continue;
                }

                var xml = await repository.GetCalendarXmlAsync(year, culture);
                _context.CalendarEntries.Add(new CalendarEntry
                {
                    Year = year,
                    Culture = culture,
                    Xml = xml
                });
            }
        }

        await _context.SaveChangesAsync();
    }
}

