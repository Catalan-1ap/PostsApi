using System.ComponentModel.DataAnnotations;


namespace Infrastructure.Options;


public sealed class PostgresOptions
{
    public const string Section = "Postgres";

    [Required]
    public string Host { get; init; } = null!;

    [Required]
    public int? Port { get; init; }

    [Required]
    public string User { get; init; } = null!;

    [Required]
    public string Password { get; init; } = null!;
}