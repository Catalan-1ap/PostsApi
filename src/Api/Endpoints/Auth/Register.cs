using Api.Common;
using Api.Responses;
using Core.Interfaces;
using Core.Models;
using FastEndpoints;
using FluentValidation;


namespace Api.Endpoints.Auth;


public sealed class RegisterRequest
{
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}


public sealed class RegisterResponse
{
    public JwtTokens Tokens { get; init; } = null!;
}


public sealed class RegisterValidator : Validator<RegisterRequest>
{
    private readonly IIdentityService _identityService;


    public RegisterValidator(IIdentityService identityService)
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


public sealed class RegisterEndpoint : Endpoint<RegisterRequest, RegisterResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;


    public RegisterEndpoint(IIdentityService identityService, IJwtService jwtService)
    {
        _identityService = identityService;
        _jwtService = jwtService;
    }


    public override void Configure()
    {
        Post(ApiRoutes.Auth.Register);
        ScopedValidator();
        AllowAnonymous();

        Summary(x =>
        {
            x.Response<LoginResponse>(StatusCodes.Status200OK, "JWT tokens");
            x.Response<ValidationErrorResponse>(StatusCodes.Status400BadRequest, "Validation error");
        });
    }


    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var id = await _identityService.Register(req.UserName, req.Email, req.Password);
        var tokens = _jwtService.Access(id);

        await SendOkAsync(new()
            {
                Tokens = tokens
            },
            ct
        );
    }
}