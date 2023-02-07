using Microsoft.Extensions.Configuration;
using NuGetInfo.CLI.Implementation;
using NuGetInfo.Client;

var builder = new ConfigurationBuilder();
builder.AddCommandLine(args);
var config = builder.Build();

//var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));
var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://azuresearch-usnc.nuget.org/");

var client = new NuGetInfoClient(httpClient);
var projects = await client.SearchPackageIdsAsync(new []
{
    "Dapper.QX",    
    "AO.Models",
    "DataTables.Library",
    "Excel2SqlServer",
    "SqlServer.LocalDb.Testing",
    "AO.Dapper.Repository.SqlServer",
    "AO.Mailgun",
    "AO.Smtp2Go"
});

var cache = new AppData();
var latestMetrics = await GetLatestPackageMetricsAsync(cache);
await CachePackageMetricsAsync(projects, cache);
var currentMetrics = CurrentDownloadMetrics(projects);
var deltas = GetDownloadCountDeltas(latestMetrics, currentMetrics);

foreach (var authorGrp in projects.GroupBy(item => item.AuthorText))
{
    Console.WriteLine();
    Console.WriteLine(string.Join(", ", authorGrp.Key));
    int maxWidth = authorGrp.Max(p => p.packageId.Length) + 2;
    int maxDownloadChars = authorGrp.Max(p => p.totalDownloads.ToString().Length) + 2;
    foreach (var prj in authorGrp.OrderBy(p => p.packageId))
    {
        var output = $"- {prj.packageId.PadRight(maxWidth, '.')}{prj.totalDownloads.ToString("n0").PadLeft(maxDownloadChars, '.')}";
        if (deltas.TryGetValue(prj.packageId, out int delta))
        {
            output += $" ( +{delta})";
        }
        Console.WriteLine(output);
    }    
}

if (config.WaitForInput())
{
    Console.WriteLine();
    Console.WriteLine("Press any key to exit.");
    Console.Read();
}

/// <summary>
/// help from https://stackoverflow.com/a/18925296/2023653
/// </summary>
class LoggingHandler : DelegatingHandler
{
    public LoggingHandler(HttpMessageHandler handler) : base(handler)
    {
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine(request.ToString());
        if (request.Content != null)
        {
            Console.WriteLine(await request.Content.ReadAsStringAsync());
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
