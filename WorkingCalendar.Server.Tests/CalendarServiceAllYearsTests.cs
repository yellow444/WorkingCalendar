using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WorkingCalendar.Application;
using WorkingCalendar.Infrastructure;

namespace WorkingCalendar.Server.Tests;

public class CalendarServiceAllYearsTests
{
    private class TestHostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Development;
    }

    [Fact]
    public async Task GetYearSqlAsync_AllYears_FileRepository()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(tempRoot, "ru", "2023"));
        Directory.CreateDirectory(Path.Combine(tempRoot, "ru", "2024"));
        await File.WriteAllTextAsync(Path.Combine(tempRoot, "ru", "2023", "calendar.xml"), "<calendar/>");
        await File.WriteAllTextAsync(Path.Combine(tempRoot, "ru", "2024", "calendar.xml"), "<calendar/>");

        var env = new TestHostEnvironment { ContentRootPath = tempRoot };
        var repoOptions = Options.Create(new CalendarRepositoryOptions { BasePath = string.Empty });
        var repo = new FileCalendarRepository(repoOptions, env);
        var service = new CalendarService(repo);

        var sql = await service.GetYearSqlAsync("all", "postgresql", 5);

        Assert.Contains("2023-01-01", sql);
        Assert.Contains("2024-01-01", sql);
    }

    [Fact]
    public async Task GetYearSqlAsync_AllYears_DbRepository()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<CalendarDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new CalendarDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.CalendarEntries.Add(new CalendarEntry { Year = 2023, Culture = "ru", Xml = "<calendar/>" });
        context.CalendarEntries.Add(new CalendarEntry { Year = 2024, Culture = "ru", Xml = "<calendar/>" });
        await context.SaveChangesAsync();

        var repo = new DbCalendarRepository(context);
        var service = new CalendarService(repo);

        var sql = await service.GetYearSqlAsync("all", "postgresql", 5);

        Assert.Contains("2023-01-01", sql);
        Assert.Contains("2024-01-01", sql);
    }
}

