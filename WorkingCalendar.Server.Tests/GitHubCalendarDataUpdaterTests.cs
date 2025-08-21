using System.IO.Compression;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using RichardSzalay.MockHttp;
using WorkingCalendar.Infrastructure;
using WorkingCalendar.Infrastructure.Services;

namespace WorkingCalendar.Server.Tests;

public class GitHubCalendarDataUpdaterTests
{
    private class TestHostEnvironment : IHostEnvironment
    {
        public string ApplicationName { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public string EnvironmentName { get; set; } = Environments.Development;
    }

    [Fact]
    public async Task UpdateDataAsync_DownloadsAndUpdates()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(tempRoot, "Data"));
            var env = new TestHostEnvironment { ContentRootPath = tempRoot };
            var options = Options.Create(new CalendarUpdateOptions
            {
                DownloadUrl = "https://api.github.com/repos/xmlcalendar/data/releases/latest",
                Countries = new[] { "ru" }
            });
            var repoOptions = Options.Create(new CalendarRepositoryOptions { BasePath = "Data" });

        // create zip content
        var archiveBytes = CreateSampleArchive();
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(options.Value.DownloadUrl)
            .Respond("application/json", "{\"zipball_url\": \"https://example.com/archive.zip\"}");
        mockHttp.When("https://example.com/archive.zip")
            .Respond(req => new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(archiveBytes)
            });
        var httpClient = mockHttp.ToHttpClient();
            var updater = new GitHubCalendarDataUpdater(httpClient, options, repoOptions, env, NullLogger<GitHubCalendarDataUpdater>.Instance);

        await updater.UpdateDataAsync(CancellationToken.None);

        Assert.True(File.Exists(Path.Combine(tempRoot, "Data", "ru", "2024", "calendar.xml")));
    }

    [Fact]
    public async Task UpdateDataAsync_HandlesErrors()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(Path.Combine(tempRoot, "Data"));
            var env = new TestHostEnvironment { ContentRootPath = tempRoot };
            var options = Options.Create(new CalendarUpdateOptions
            {
                DownloadUrl = "https://api.github.com/repos/xmlcalendar/data/releases/latest",
                Countries = new[] { "ru" }
            });
            var repoOptions = Options.Create(new CalendarRepositoryOptions { BasePath = "Data" });
        var mockHttp = new MockHttpMessageHandler();
        mockHttp.When(options.Value.DownloadUrl)
            .Respond(System.Net.HttpStatusCode.InternalServerError);
        var httpClient = mockHttp.ToHttpClient();
            var updater = new GitHubCalendarDataUpdater(httpClient, options, repoOptions, env, NullLogger<GitHubCalendarDataUpdater>.Instance);

        await updater.UpdateDataAsync(CancellationToken.None);

        Assert.False(File.Exists(Path.Combine(tempRoot, "Data", "ru", "2024", "calendar.xml")));
    }

    private static byte[] CreateSampleArchive()
    {
        using var ms = new MemoryStream();
        using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            var entry = archive.CreateEntry("xmlcalendar-data/ru/2024/calendar.xml");
            using (var writer = new StreamWriter(entry.Open()))
            {
                writer.Write("test");
            }
            archive.CreateEntry("xmlcalendar-data/calendar.xsd");
        }
        return ms.ToArray();
    }
}
