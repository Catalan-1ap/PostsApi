using Api.Common;
using Api.Processors;
using Api.Responses;
using Core.Interfaces;
using Core.Models;
using FastEndpoints;
using FluentValidation;


namespace Api.Endpoints.Auth;


public sealed class LoginRequest
{
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
}


public sealed class LoginResponse
{
    public JwtTokens Tokens { get; init; } = null!;
}


public sealed class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}


public sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;


    public LoginEndpoint(IIdentityService identityService, IJwtService jwtService)
    {
        _identityService = identityService;
        _jwtService = jwtService;
    }


    public override void Configure()
    {
        Post(ApiRoutes.Auth.Login);
        AllowAnonymous();
        PostProcessors(new SaveChangesPostProcessor<LoginRequest, LoginResponse>());
        
        Summary(x =>
        {
            x.Response<LoginResponse>(StatusCodes.Status200OK, "JWT tokens");
            x.Response<SingleErrorResponse>(StatusCodes.Status400BadRequest, "Email/Password combination is wrong");
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound, "User does not exists");
        });
    }


    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _identityService.LoginAsync(req.Email, req.Password);

        var tokens = _jwtService.Access(user);

        await SendOkAsync(new()
            {
                Tokens = tokens
            },
            ct
        );
    }
}