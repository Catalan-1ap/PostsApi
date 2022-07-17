using System.Text;
using Api.Common;
using Application.Interfaces;
using Application.Settings;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace Api.Installers;


public static class JwtInstaller
{


    public static void InstallJwt(this IServiceCollection services, ConfigurationManager configuration)
    {
        var envs = new[]
        {
            "JWT_SECRET",
            "JWT_ISSUER",
            "JWT_AUDIENCE"
        }.GetEnvironmentVariables();
        var secret = envs["JWT_SECRET"];
        var issuer = envs["JWT_ISSUER"];
        var audience = envs["JWT_AUDIENCE"];
        var keyBytes = Encoding.ASCII.GetBytes(secret);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var expiresOptions = configuration.GetSection(nameof(Expires)).Get<Expires>();

        services.AddSingleton<IJwtSettings>(provider =>
            new JwtSettings(provider.GetRequiredService<IDateTimeService>())
            {
                Issuer = issuer,
                Audience = audience,
                Credentials = credentials,
                ExpiresForAccessTokenInput = TimeSpan.Parse(expiresOptions.AccessToken),
                ExpiresForRefreshTokenInput = TimeSpan.Parse(expiresOptions.RefreshToken)
            });

        services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.SaveToken = true;
                x.TokenValidationParameters = new()
                {
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = securityKey,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }


    private sealed class Expires
    {
        public string? AccessToken { get; init; }
        public string? RefreshToken { get; init; }
    }
}