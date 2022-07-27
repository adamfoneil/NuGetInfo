using Microsoft.Extensions.Configuration;

internal static class Extensions
{
    internal static bool WaitForInput(this IConfiguration config) => config["pause"]?.Equals("true") ?? false;
}
