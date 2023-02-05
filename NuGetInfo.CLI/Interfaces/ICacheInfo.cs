namespace NuGetInfo.CLI.Interfaces
{
    internal interface ICacheInfo
    {
        DateOnly CurrentDate { get; }
        string LocalPath { get; }        
    }
}
