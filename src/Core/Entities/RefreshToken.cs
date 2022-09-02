namespace Core.Entities;


public sealed class RefreshToken : IAuditable
{
    public string Token { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateOnly ExpiredAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}