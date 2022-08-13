namespace Core.Models;


public sealed class JwtTokens
{
    public string Access { get; init; } = null!;
    public string Refresh { get; init; } = null!;
}