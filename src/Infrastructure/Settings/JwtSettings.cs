using Application.Interfaces;
using Application.Settings;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Settings;


public sealed class JwtSettings : IJwtSettings
{
    private readonly IDateTimeService _dateTimeService;

    public TimeSpan ExpiresForAccessTokenInput { get; init; }
    public TimeSpan ExpiresForRefreshTokenInput { get; init; }

    public string? Issuer { get; init; }
    public string? Audience { get; init; }
    public SigningCredentials? Credentials { get; init; }
    public DateTime ExpiresForAccessToken => _dateTimeService.UtcNow.Add(ExpiresForAccessTokenInput);
    public DateTime ExpiresForRefreshToken => _dateTimeService.UtcNow.Add(ExpiresForRefreshTokenInput);

    public JwtSettings(IDateTimeService dateTimeService) => _dateTimeService = dateTimeService;
}