using NuGetInfo.CLI.Interfaces;
using NuGetInfo.Client.Models;
using System.Text.Json;

internal partial class Program
{
    const string CacheRefFile = "latest.json";

    /// <summary>
    /// saves the current download stats for each package with today's timestamp (if not done already)    
    /// </summary>
    static async Task CachePackageMetricsAsync(IEnumerable<Project> projects, ICacheInfo cacheInfo)
    {
        var fileName = $"package-stats-{cacheInfo.CurrentDate:yyyy-MM-dd}.json";
        var outputFile = Path.Combine(cacheInfo.LocalPath, fileName);

        var folder = Path.GetDirectoryName(outputFile);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder!);

        // if today's cache has already been written, don't overwrite
        if (File.Exists(outputFile)) return;

        var data = CurrentDownloadMetrics(projects);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = true });
        await File.WriteAllTextAsync(outputFile, json);
        
        if (cacheInfo.CurrentDate > (await GetMarkerAsync(cacheInfo)).Date)
        {
            json = JsonSerializer.Serialize(new DateMarker()
            {
                Filename = fileName,
                Date = cacheInfo.CurrentDate
            }, new JsonSerializerOptions() { WriteIndented = true });

            await File.WriteAllTextAsync(Path.Combine(cacheInfo.LocalPath, CacheRefFile), json);
        }
    }

    static async Task<DateMarker> GetMarkerAsync(ICacheInfo cacheInfo)
    {
        var markerFile = Path.Combine(cacheInfo.LocalPath, CacheRefFile);
        if (File.Exists(markerFile))
        {
            var json = await File.ReadAllTextAsync(markerFile);
            var info = JsonSerializer.Deserialize<DateMarker>(json) ?? throw new Exception("Error deserializing date marker.");
            return info;
        }

        return await Task.FromResult(new DateMarker());
    }


    static async Task<Dictionary<string, int>> GetLatestPackageMetricsAsync(ICacheInfo cacheInfo)
    {
        var marker = await GetMarkerAsync(cacheInfo);
        var fileName = Path.Combine(cacheInfo.LocalPath, marker.Filename);

        if (File.Exists(fileName))
        {
            var json = await File.ReadAllTextAsync(fileName);
            var result = JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? throw new Exception("Error deserializing cache data");
            return result;
        }

        return new();
    }

    static Dictionary<string, int> GetDownloadCountDeltas(Dictionary<string, int> latestMetrics, Dictionary<string, int> currentMetrics) =>
        latestMetrics.Join(currentMetrics, 
            kp => kp.Key, kp => kp.Key, 
            (current, latest) => new
            {
                PackageId = current.Key,
                Delta = latest.Value - current.Value
            })
            .Where(d => d.Delta > 0)
            .ToDictionary(item => item.PackageId, item => item.Delta);
    
    private class DateMarker
    {
        public string Filename { get; set; } = default!;
        public DateOnly Date { get; set; }
    }

    static Dictionary<string, int> CurrentDownloadMetrics(IEnumerable<Project> projects) => 
        projects.ToDictionary(p => p.packageId, p => p.totalDownloads);
}

