using NuGetInfo.Client;

var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));
httpClient.BaseAddress = new Uri("https://azuresearch-usnc.nuget.org/");

var client = new NuGetInfoClient(httpClient);
var result = await client.SearchManyAsync(new []
{
    "dapper.qx",
    "ao.dapper.repository",
    "ao.models",
    "datatables.library",
    "excel2sqlserver"
});

foreach (var project in result.SelectMany(sr => sr.data.Select(prj => new
{
    prj.packageId,
    prj.totalDownloads
})))
{
    Console.WriteLine($"{project.packageId}, total downloads {project.totalDownloads:n0}");
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