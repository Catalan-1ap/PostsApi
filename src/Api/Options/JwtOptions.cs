using System.ComponentModel.DataAnnotations;


namespace Api.Options;


public sealed class JwtOptions
{
    public const string Section = "Jwt";

    [Required]
    public string Secret { get; init; } = null!;

    [Required]
    public string Issuer { get; init; } = null!;
}