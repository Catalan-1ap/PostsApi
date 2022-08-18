namespace Core.Models;


public sealed class CoreEnvironment
{
    public Uri BaseUri { get; init; }
    public string StaticRootFsPath { get; init; }
    public string AvatarsFsPath { get; init; }
    public string CoversFsPath { get; init; }
}