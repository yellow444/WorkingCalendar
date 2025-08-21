using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WorkingCalendar.Infrastructure;

namespace WorkingCalendar.Server.Tests;

public class FileCalendarRepositoryTests
{
    private class TestHostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Development;
    }

    [Fact]
    public async Task GetCalendarXmlAsync_ReturnsContent()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(tempRoot, "ru", "2024"));
        await File.WriteAllTextAsync(Path.Combine(tempRoot, "ru", "2024", "calendar.xml"), "<calendar/>");
        var env = new TestHostEnvironment { ContentRootPath = tempRoot };
        var repoOptions = Options.Create(new CalendarRepositoryOptions { BasePath = "" });
        var repo = new FileCalendarRepository(repoOptions, env);
        var xml = await repo.GetCalendarXmlAsync(2024, "ru");
        Assert.Contains("<calendar", xml);
    }
}
