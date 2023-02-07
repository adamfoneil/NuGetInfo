using NuGetInfo.CLI.Interfaces;
using NuGetInfo.CLI.Static;
using NuGetInfo.Client.Models;
using System.Text.Json;

internal partial class Program
{
    /// <summary>
    /// saves the current download stats for each package with today's timestamp (if not done already)    
    /// </summary>
    static async Task<bool> CacheDownloadCountsAsync(IEnumerable<Project> projects, ICacheInfo cacheInfo)
    {
        var fileName = $"downloads-{cacheInfo.CurrentDate:yyyy-MM-dd}.json";
        var outputFile = Path.Combine(cacheInfo.LocalPath, fileName);

        var folder = Path.GetDirectoryName(outputFile);
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder!);

        // if today's cache has already been written, don't overwrite
        if (File.Exists(outputFile)) return false;

        var data = projects.ToDictionary(p => p.packageId, p => p.totalDownloads);
        var json = JsonSerializer.Serialize(data, new JsonSerializerOptions() { WriteIndented = true });
        await File.WriteAllTextAsync(outputFile, json);

        return true;        
    }

    static IEnumerable<(DateOnly Date, string Path)> GetCacheFiles(ICacheInfo cacheInfo)
    {
        var files = Directory.GetFiles(cacheInfo.LocalPath, "downloads-*.json", SearchOption.TopDirectoryOnly);

        return files.Select(name => (Parse.DateFromPath(name), name));        
    }

    static async Task<(DateOnly PriorDate, Dictionary<string, int> PriorDownloads, Dictionary<string, int> LatestDownloads)> GetComparisonFilesAsync(IEnumerable<(DateOnly Date, string Path)> files) 
    {
        var twoMostRecent = files.OrderByDescending(info => info.Date).Take(2).ToArray();
        
        return 
        (
            twoMostRecent[1].Date,
            await GetDictionaryAsync(twoMostRecent[1].Path),
            await GetDictionaryAsync(twoMostRecent[0].Path)
        );
        
        async Task<Dictionary<string, int>> GetDictionaryAsync(string fileName)
        {
            var json = await File.ReadAllTextAsync(fileName);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? throw new Exception("Error deserializing cache data");            
        }
    }

    static Dictionary<string, int> GetDownloadCountDeltas(Dictionary<string, int> priorDownloads, Dictionary<string, int> currentDownloads) =>
        priorDownloads.Join(currentDownloads, 
            kp => kp.Key, kp => kp.Key, 
            (current, latest) => new
            {
                PackageId = current.Key,
                Delta = latest.Value - current.Value
            })
            .Where(d => d.Delta > 0)
            .ToDictionary(item => item.PackageId, item => item.Delta);    
}

