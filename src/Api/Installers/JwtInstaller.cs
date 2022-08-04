using System.ComponentModel.DataAnnotations;
using System.Text;
using Api.Common;
using Core.Interfaces;
using Core.Settings;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;


namespace Api.Installers;


public static class JwtInstaller
{
    public static void AddJwt(this IServiceCollection services, ConfigurationManager configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.Section).Get<JwtOptions>();
        var expiresOptions = configuration.GetSection(ExpiresOptions.Section).Get<ExpiresOptions>();

        jwtOptions.ValidateDataAnnotations();
        expiresOptions.ValidateDataAnnotations();

        var keyBytes = Encoding.ASCII.GetBytes(jwtOptions.Secret);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

        services.AddSingleton<IJwtSettings>(provider =>
            new JwtSettings(
                provider.GetRequiredService<IDateTimeService>(),
                jwtOptions.Issuer,
                jwtOptions.Audience,
                credentials,
                expiresOptions.AccessToken!.Value,
                expiresOptions.RefreshToken!.Value
            )
        );

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
                    IssuerSigningKey = securityKey,
                    ClockSkew = TimeSpan.Zero
                };
            });
    }


    private sealed class JwtOptions
    {
        public const string Section = "Jwt";

        [Required]
        public string Secret { get; init; } = null!;

        [Required]
        public string Issuer { get; init; } = null!;

        [Required]
        public string Audience { get; init; } = null!;
    }


    private sealed class ExpiresOptions
    {
        public const string Section = "Expires";

        [Required]
        public TimeSpan? AccessToken { get; init; }

        [Required]
        public TimeSpan? RefreshToken { get; init; }
    }
}