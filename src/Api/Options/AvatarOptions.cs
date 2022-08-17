using System.ComponentModel.DataAnnotations;


namespace Api.Options;


public sealed class AvatarOptions
{
    public const string Section = "Avatar";

    [Required]
    public int MaxSize { get; init; }

    [Required]
    public string[] PossibleExtensions { get; init; } = null!;

    [Required]
    public int MaxWidth { get; init; }

    [Required]
    public int MaxHeight { get; init; }
}