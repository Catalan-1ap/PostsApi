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

        // 
        var keyBytes = Convert.FromBase64String(jwtOptions.Secret);
        var securityKey = new SymmetricSecurityKey(keyBytes);
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero,
        };

        var serviceValidationParameters = tokenValidationParameters.Clone();
        serviceValidationParameters.ValidateLifetime = false;
        services.AddSingleton<IJwtSettings>(provider =>
            new JwtSettings(
                provider.GetRequiredService<IDateTimeService>(),
                jwtOptions.Issuer,
                credentials,
                serviceValidationParameters,
                expiresOptions.AccessToken!.Value,
                expiresOptions.RefreshToken!.Value
            )
        );
        services.AddSingleton(serviceValidationParameters);
        
        services.AddAuthentication(x =>
            {
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.TokenValidationParameters = tokenValidationParameters;
            });
    }


    private sealed class JwtOptions
    {
        public const string Section = "Jwt";

        [Required]
        public string Secret { get; init; } = null!;

        [Required]
        public string Issuer { get; init; } = null!;
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