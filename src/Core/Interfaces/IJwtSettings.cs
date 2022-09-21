using Microsoft.IdentityModel.Tokens;


namespace Core.Interfaces;


public interface IJwtSettings
{
    string Issuer { get; }

    SigningCredentials Credentials { get; }

    public TokenValidationParameters TokenValidationParameters { get; }

    DateTime ExpiresForAccessToken { get; }

    DateTime ExpiresForRefreshToken { get; }
}