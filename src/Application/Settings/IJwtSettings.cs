using Microsoft.IdentityModel.Tokens;


namespace Application.Settings;


public interface IJwtSettings
{
    string Issuer { get; }

    string Audience { get; }

    SigningCredentials Credentials { get; }

    DateTime ExpiresForAccessToken { get; }

    DateTime ExpiresForRefreshToken { get; }
}