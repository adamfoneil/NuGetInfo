using NuGetInfo.CLI.Interfaces;

namespace NuGetInfo.CLI.Implementation
{
    internal class AppData : ICacheInfo
    {
        public DateOnly CurrentDate => DateOnly.FromDateTime(DateTime.Today);

        public string LocalPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "NuGetInfo.CLI");
    }
}
