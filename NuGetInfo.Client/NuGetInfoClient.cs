using NuGetInfo.Client.Interfaces;
using NuGetInfo.Client.Models;
using Refit;

namespace NuGetInfo.Client
{
    public class NuGetInfoClient
    {
        private readonly INuGetClient _api;

        public NuGetInfoClient(HttpClient httpClient)
        {
            _api = RestService.For<INuGetClient>(httpClient);
        }

        public NuGetInfoClient()
        {
            _api = RestService.For<INuGetClient>("https://azuresearch-usnc.nuget.org/");
        }

        public async Task<SearchResults> SearchAsync(string query) => await _api.SearchAsync(query);

        public async Task<IEnumerable<SearchResults>> SearchManyAsync(IEnumerable<string> queries)
        {
            List<SearchResults> results = new();

            foreach (var query in queries)
            {
                var result = await _api.SearchAsync(query);
                results.Add(result);
            }

            return results;
        }
    }
}
