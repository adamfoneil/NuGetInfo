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

        public async Task<IEnumerable<Project>> SearchProjectsAsync(string query) => (await _api.SearchAsync(query)).data;

        public async Task<IEnumerable<Project>> SearchManyAsync(IEnumerable<string> queries)
        {
            List<Project> results = new();

            foreach (var query in queries)
            {
                var projects = await SearchProjectsAsync(query);
                results.AddRange(projects);
            }

            return results;
        }

        public async Task<IEnumerable<Project>> SearchPackageIdsAsync(IEnumerable<string> packageIds) =>
            (await SearchManyAsync(packageIds)).Where(prj => packageIds.Contains(prj.packageId));        
    }
}
