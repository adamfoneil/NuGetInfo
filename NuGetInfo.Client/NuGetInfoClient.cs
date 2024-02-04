using NuGetInfo.Client.Interfaces;
using NuGetInfo.Client.Models;
using Refit;

namespace NuGetInfo.Client
{
    public class NuGetInfoClient
    {
        public const string PrimaryUrl = "https://azuresearch-usnc.nuget.org/";
        public const string SecondaryUrl = "https://azuresearch-usnc.nuget.org/";
        public IEnumerable<string> Urls = new[] { PrimaryUrl, SecondaryUrl };

        public static Uri BaseUri => new(PrimaryUrl);

		private readonly INuGetClient _apiPrimary;
        private readonly INuGetClient _apiSecondary;

        public NuGetInfoClient(HttpClient httpClient)
        {
            _apiPrimary = RestService.For<INuGetClient>(httpClient);
            // note this doesn't work because it should be using the secondary url
            _apiSecondary = RestService.For<INuGetClient>(httpClient);
        }

        public NuGetInfoClient()
        {
            _apiPrimary = RestService.For<INuGetClient>(PrimaryUrl);
            _apiSecondary = RestService.For<INuGetClient>(SecondaryUrl);
        }

        public async Task<IEnumerable<Project>> SearchProjectsAsync(string query)
        {
            var results1 = (await _apiPrimary.SearchAsync(query)).data;
            var results2 = (await _apiSecondary.SearchAsync(query)).data;

            return results1;
		}
            

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
