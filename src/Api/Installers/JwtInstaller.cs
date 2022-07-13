using System.Text;
using Api.Common;
using Application.Options;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace Api.Installers;


public static class JwtInstaller
{
    public static void InstallJwt(this IServiceCollection services)
    {
        var (jwtOptions, key) = GetJwtOptions();

        services.AddSingleton<IJwtOptions>(jwtOptions);

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
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }


    private static (JwtOptions jwtOptions, SecurityKey key) GetJwtOptions()
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
        var (key, credentials) = GetSigningDetails(secret);

        return (new(issuer, audience, credentials), key);
    }


    private static (SecurityKey key, SigningCredentials credentials) GetSigningDetails(string secret)
    {
        var keyBytes = Encoding.ASCII.GetBytes(secret);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        return (securityKey, credentials);
    }
}