using Application.Interfaces;
using Application.Models;
using FluentValidation;
using MediatR;


namespace Application.Features;


public sealed record RegisterRequest(string UserName, string Email, string Password) : IRequest<RegisterResponse>;


public sealed record RegisterResponse(JwtTokens Tokens);


public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).EmailAddress();
    }
}


internal sealed class RegisterHandler : IRequestHandler<RegisterRequest, RegisterResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;


    public RegisterHandler(IIdentityService identityService, IJwtService jwtService)
    {
        _identityService = identityService;
        _jwtService = jwtService;
    }


    public async Task<RegisterResponse> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var (userName, email, password) = request;

        var id = await _identityService.Register(userName, email, password);
        var tokens = _jwtService.Access(id);

        return new(tokens);
    }
}