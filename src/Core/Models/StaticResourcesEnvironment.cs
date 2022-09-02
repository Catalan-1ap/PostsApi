namespace Core.Models;


public sealed class StaticResourcesEnvironment
{
    public string StaticRootFsPath { get; init; } = null!;
    public string AvatarsFsPath { get; init; } = null!;
    public string CoversFsPath { get; init; } = null!;
}