using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Interfaces;
using Application.Options;
using FluentValidation;
using MediatR;


namespace Application.Features;


public sealed record RegisterRequest(string UserName, string Email, string Password) : IRequest<RegisterResponse>;


public sealed record RegisterResponse(string JwtToken);


public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}


internal sealed class RegisterHandler : IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IIdentityService _identityService;
    private readonly IJwtOptions _jwtOptions;


    public RegisterHandler(IIdentityService identityService,
                           IDateTimeService dateTimeService,
                           IJwtOptions jwtOptions)
    {
        _identityService = identityService;
        _dateTimeService = dateTimeService;
        _jwtOptions = jwtOptions;
    }


    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var (userName, email, password) = request;

        var id = await _identityService.Register(userName, email, password);
        var token = CreateJwtToken(id);

        return new(token);
    }


    private string CreateJwtToken(string userId)
    {
        var securityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: CreateClaimsForUser(userId),
            signingCredentials: _jwtOptions.Credentials,
            expires: _jwtOptions.Expires(_dateTimeService),
            notBefore: _dateTimeService.UtcNow
        );

        var handler = new JwtSecurityTokenHandler();
        var token = handler.WriteToken(securityToken);

        return token;
    }


    private IEnumerable<Claim> CreateClaimsForUser(string userId)
    {
        return new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
    }
}