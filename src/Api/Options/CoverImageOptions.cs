using System.ComponentModel.DataAnnotations;
using Core.Models;


namespace Api.Options;


public sealed class CoverImageOptions
{
    public const string Section = "CoverImage";

    [Required]
    public int MaxSize { get; init; }

    [Required]
    public string[] PossibleExtensions { get; init; } = null!;

    [Required]
    public ImageSize MaxResolution { get; init; } = null!;
}