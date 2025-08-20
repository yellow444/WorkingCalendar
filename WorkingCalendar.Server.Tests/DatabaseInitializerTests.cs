using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WorkingCalendar.Infrastructure;
using WorkingCalendar.Server;

namespace WorkingCalendar.Server.Tests;

public class DatabaseInitializerTests
{
    private class TestHostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Development;
    }

    [Fact]
    public async Task InitializeAsync_PopulatesDatabaseFromXml_WhenEmpty()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<CalendarDbContext>()
            .UseSqlite(connection)
            .Options;
        await using var context = new CalendarDbContext(options);

        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(tempRoot, "ru", "2024"));
        await File.WriteAllTextAsync(Path.Combine(tempRoot, "ru", "2024", "calendar.xml"), "<calendar/>");
        var env = new TestHostEnvironment();
        var repoOptions = Options.Create(new CalendarRepositoryOptions { BasePath = tempRoot });
        var initializer = new DatabaseInitializer(context, repoOptions, env);

        await initializer.InitializeAsync();

        var entry = await context.CalendarEntries.SingleAsync();
        Assert.Equal(2024, entry.Year);
        Assert.Equal("ru", entry.Culture);
        Assert.Contains("<calendar", entry.Xml);
    }
}
