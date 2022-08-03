namespace Core.Entities;


public sealed class RefreshToken
{
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateOnly ExpiredAt { get; set; }
    public DateOnly CreatedAt { get; set; }
}