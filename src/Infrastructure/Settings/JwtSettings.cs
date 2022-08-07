using Core.Interfaces;
using Core.Settings;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Settings;


public sealed class JwtSettings : IJwtSettings
{
    private readonly IDateTimeService _dateTimeService;
    private readonly TimeSpan _expiresForAccessTokenInput;
    private readonly TimeSpan _expiresForRefreshTokenInput;

    public string Issuer { get; }
    public SigningCredentials Credentials { get; }
    public TokenValidationParameters TokenValidationParameters { get; }
    public DateTime ExpiresForAccessToken => _dateTimeService.UtcNow.Add(_expiresForAccessTokenInput);
    public DateTime ExpiresForRefreshToken => _dateTimeService.UtcNow.Add(_expiresForRefreshTokenInput);


    public JwtSettings(
        IDateTimeService dateTimeService,
        string issuer,
        SigningCredentials credentials,
        TokenValidationParameters tokenValidationParameters,
        TimeSpan expiresForAccessTokenInput,
        TimeSpan expiresForRefreshTokenInput
    )
    {
        _dateTimeService = dateTimeService;
        Issuer = issuer;
        Credentials = credentials;
        _expiresForAccessTokenInput = expiresForAccessTokenInput;
        _expiresForRefreshTokenInput = expiresForRefreshTokenInput;
        TokenValidationParameters = tokenValidationParameters;
    }
}