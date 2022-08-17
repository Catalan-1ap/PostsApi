using System.ComponentModel.DataAnnotations;


namespace Api.Options;


public sealed class ExpiresOptions
{
    public const string Section = "Expires";

    [Required]
    public TimeSpan? AccessToken { get; init; }

    [Required]
    public TimeSpan? RefreshToken { get; init; }
}