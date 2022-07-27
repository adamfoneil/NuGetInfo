using NuGetInfo.Client;

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

foreach (var authorGrp in projects.GroupBy(item => item.AuthorText))
{
    Console.WriteLine();
    Console.WriteLine(string.Join(", ", authorGrp.Key));
    foreach (var prj in authorGrp.OrderBy(p => p.packageId))
    {
        Console.WriteLine($"- {prj.packageId}, total downloads {prj.totalDownloads:n0}");
    }    
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