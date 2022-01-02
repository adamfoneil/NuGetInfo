using NuGetInfo.Client.Models;
using Refit;

namespace NuGetInfo.Client.Interfaces
{
    internal interface INuGetClient
    {
        [Get("/query?q={query}")]
        Task<SearchResults> SearchAsync(string query);
    }
}
