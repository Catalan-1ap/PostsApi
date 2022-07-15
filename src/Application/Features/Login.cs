using Application.Interfaces;
using Application.Models;
using FluentValidation;
using MediatR;


namespace Application.Features;


public sealed record LoginRequest(string Email, string Password) : IRequest<LoginResponse>;


public sealed record LoginResponse(JwtTokens Tokens);


public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}


internal sealed class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;


    public LoginHandler(IIdentityService identityService, IJwtService jwtService)
    {
        _identityService = identityService;
        _jwtService = jwtService;
    }


    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _identityService.Login(request.Email, request.Password);

        var tokens = _jwtService.Access(user.Id);

        return new(tokens);
    }
}