using Core.Interfaces;
using Core.Models;
using FluentValidation;
using MediatR;


namespace Core.Features.Auth;


public sealed record RegisterRequest(string UserName, string Email, string Password) : IRequest<RegisterResponse>;


public sealed record RegisterResponse(JwtTokens Tokens);


public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    private readonly IIdentityService _identityService;


    public RegisterRequestValidator(IIdentityService identityService)
    {
        _identityService = identityService;

        RuleFor(x => x.UserName)
            .MustAsync(UniqueUsername).WithMessage("Must be unique");

        RuleFor(x => x.Email)
            .EmailAddress()
            .MustAsync(UniqueEmail).WithMessage("Must be unique");

        RuleFor(x => x.Password)
            .NotEmptyWithMessage()
            .MinimumLengthWithMessage(8);
    }


    private async Task<bool> UniqueUsername(string userName, CancellationToken cancellationToken) =>
        await _identityService.IsUsernameUnique(userName);


    private async Task<bool> UniqueEmail(string email, CancellationToken cancellationToken) =>
        await _identityService.IsEmailUnique(email);
}


public sealed class RegisterHandler : IRequestHandler<RegisterRequest, RegisterResponse>
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