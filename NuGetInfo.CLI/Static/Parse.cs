namespace NuGetInfo.CLI.Static
{
    public static class Parse
    {
        public static DateOnly DateFromPath(string path)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var dateStr = name.Substring("downloads-".Length);
            var parts = dateStr.Split('-');
            return new DateOnly(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
        }
    }
}
