using Application.Interfaces;
using Application.Options;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Options;


public sealed record JwtOptions(string Issuer, string Audience, SigningCredentials Credentials) : IJwtOptions
{
    public DateTime Expires(IDateTimeService dateTimeService) => dateTimeService.UtcNow.AddMinutes(2);
}