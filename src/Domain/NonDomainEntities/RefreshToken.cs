namespace Domain.NonDomainEntities;


public sealed class RefreshToken
{
    public string Token { get; private set; }
    public string UserId { get; private set; }
    public DateOnly ExpiredAt { get; private set; }
    public DateOnly CreatedAt { get; private set; }


    public RefreshToken(string token, string userId, DateOnly expiredAt, DateOnly createdAt)
    {
        Token = token;
        UserId = userId;
        ExpiredAt = expiredAt;
        CreatedAt = createdAt;
    }
}