using Application.Interfaces;
using Application.Settings;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Settings;


public sealed class JwtSettings : IJwtSettings
{
    private readonly IDateTimeService _dateTimeService;

    public TimeSpan ExpiresForAccessTokenInput { get; set; }
    public TimeSpan ExpiresForRefreshTokenInput { get; set; }

    public string Issuer { get; set; }
    public string Audience { get; set; }
    public SigningCredentials Credentials { get; set; }
    public DateTime ExpiresForAccessToken => _dateTimeService.UtcNow.Add(ExpiresForAccessTokenInput);
    public DateTime ExpiresForRefreshToken => _dateTimeService.UtcNow.Add(ExpiresForRefreshTokenInput);

    public JwtSettings(IDateTimeService dateTimeService) => _dateTimeService = dateTimeService;
}