using System.Text.Json.Serialization;

#nullable disable

namespace NuGetInfo.Client.Models
{

    public class SearchResults
    {
        public Context context { get; set; }
        public int totalHits { get; set; }
        public Project[] data { get; set; }
    }

    public class Context
    {
        public string vocab { get; set; }
        public string _base { get; set; }
    }

    public class Project
    {
        [JsonPropertyName("@id")]
        public string id { get; set; }
        public string type { get; set; }
        public string registration { get; set; }
        [JsonPropertyName("id")]
        public string packageId { get; set; }
        public string version { get; set; }
        public string description { get; set; }
        public string summary { get; set; }
        public string title { get; set; }
        public string licenseUrl { get; set; }
        public string projectUrl { get; set; }
        public string[] tags { get; set; }
        public string[] authors { get; set; }
        public string[] owners { get; set; }
        public int totalDownloads { get; set; }
        public bool verified { get; set; }
        public Packagetype[] packageTypes { get; set; }
        public Version[] versions { get; set; }
    }

    public class Packagetype
    {
        public string name { get; set; }
    }

    public class Version
    {
        public string version { get; set; }
        public int downloads { get; set; }
        public string id { get; set; }
    }
}
