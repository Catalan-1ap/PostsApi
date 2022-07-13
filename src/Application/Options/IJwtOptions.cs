using Application.Interfaces;
using Microsoft.IdentityModel.Tokens;


namespace Application.Options;


public interface IJwtOptions
{
    string Issuer { get; }

    string Audience { get; }

    SigningCredentials Credentials { get; }

    DateTime Expires(IDateTimeService dateTimeService);
}