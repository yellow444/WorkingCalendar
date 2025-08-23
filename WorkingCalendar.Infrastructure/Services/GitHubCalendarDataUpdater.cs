using System.IO.Compression;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using WorkingCalendar.Infrastructure;

namespace WorkingCalendar.Infrastructure.Services
{
    public interface ICalendarDataUpdater
    {
        Task UpdateDataAsync(CancellationToken cancellationToken);
    }

    public class GitHubCalendarDataUpdater : ICalendarDataUpdater
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<GitHubCalendarDataUpdater> _logger;
        private readonly CalendarUpdateOptions _options;
        private readonly string _dataPath;

        public GitHubCalendarDataUpdater(HttpClient httpClient, IOptions<CalendarUpdateOptions> options, IOptions<CalendarRepositoryOptions> repoOptions, IHostEnvironment env, ILogger<GitHubCalendarDataUpdater> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
            var basePath = Path.IsPathRooted(repoOptions.Value.BasePath)
                ? repoOptions.Value.BasePath
                : Path.Combine(env.ContentRootPath, repoOptions.Value.BasePath);
            _dataPath = basePath;
        }

        public async Task UpdateDataAsync(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Starting calendar data update");
                using var releaseResponse = await _httpClient.GetAsync(_options.DownloadUrl, cancellationToken);
                releaseResponse.EnsureSuccessStatusCode();
                await using var releaseStream = await releaseResponse.Content.ReadAsStreamAsync(cancellationToken);
                using var json = await JsonDocument.ParseAsync(releaseStream, cancellationToken: cancellationToken);
                if (!json.RootElement.TryGetProperty("zipball_url", out var zipUrlElement))
                {
                    _logger.LogError("zipball_url not found in release metadata");
                    return;
                }
                var zipUrl = zipUrlElement.GetString();
                if (zipUrl == null)
                {
                    _logger.LogError("zipball_url is null");
                    return;
                }
                using var zipResponse = await _httpClient.GetAsync(zipUrl, cancellationToken);
                zipResponse.EnsureSuccessStatusCode();
                var tempZip = Path.GetTempFileName();
                await using (var fs = File.Create(tempZip))
                {
                    await zipResponse.Content.CopyToAsync(fs, cancellationToken);
                }
                var extractDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                ZipFile.ExtractToDirectory(tempZip, extractDir);
                var rootDir = Directory.GetDirectories(extractDir).Single();
                foreach (var country in _options.Countries)
                {
                    var source = Path.Combine(rootDir, country);
                    if (!Directory.Exists(source))
                    {
                        _logger.LogWarning("Country {Country} not found in archive", country);
                        continue;
                    }
                    var dest = Path.Combine(_dataPath, country);
                    if (Directory.Exists(dest))
                    {
                        Directory.Delete(dest, true);
                    }
                    CopyDirectory(source, dest);
                }
                var xsdSource = Path.Combine(rootDir, "calendar.xsd");
                if (File.Exists(xsdSource))
                {
                    File.Copy(xsdSource, Path.Combine(_dataPath, "calendar.xsd"), true);
                }
                Directory.Delete(extractDir, true);
                File.Delete(tempZip);
                _logger.LogInformation("Calendar data update finished");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update calendar data");
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(sourceDir, dir);
                Directory.CreateDirectory(Path.Combine(destinationDir, rel));
            }
            foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                var rel = Path.GetRelativePath(sourceDir, file);
                var dest = Path.Combine(destinationDir, rel);
                Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
                File.Copy(file, dest, true);
            }
        }
    }
}
