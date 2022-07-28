using Core.Interfaces;
using Core.Settings;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Settings;


public sealed class JwtSettings : IJwtSettings
{
    private readonly IDateTimeService _dateTimeService;
    private readonly TimeSpan _expiresForAccessTokenInput;
    private readonly TimeSpan _expiresForRefreshTokenInput;

    public string Issuer { get; init; }
    public string Audience { get; init; }
    public SigningCredentials Credentials { get; init; }
    public DateTime ExpiresForAccessToken => _dateTimeService.UtcNow.Add(_expiresForAccessTokenInput);
    public DateTime ExpiresForRefreshToken => _dateTimeService.UtcNow.Add(_expiresForRefreshTokenInput);


    public JwtSettings(IDateTimeService dateTimeService,
                       string issuer,
                       string audience,
                       SigningCredentials credentials,
                       TimeSpan expiresForAccessTokenInput,
                       TimeSpan expiresForRefreshTokenInput)
    {
        _dateTimeService = dateTimeService;
        Issuer = issuer;
        Audience = audience;
        Credentials = credentials;
        _expiresForAccessTokenInput = expiresForAccessTokenInput;
        _expiresForRefreshTokenInput = expiresForRefreshTokenInput;
    }
}