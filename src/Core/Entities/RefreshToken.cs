namespace Core.Entities;


public sealed class RefreshToken
{
    public string Token { get; set; }
    public string UserId { get; set; }
    public DateOnly ExpiredAt { get; set; }
    public DateOnly CreatedAt { get; set; }
}