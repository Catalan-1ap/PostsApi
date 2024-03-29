﻿using Api.Responses;
using Core;
using Core.Interfaces;
using Core.Models;
using FastEndpoints;
using FluentValidation;


namespace Api.Endpoints.Users;


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


public sealed class LoginEndpoint : SharedBaseEndpoint<LoginRequest, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;

    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public LoginEndpoint(IIdentityService identityService, IJwtService jwtService)
    {
        _identityService = identityService;
        _jwtService = jwtService;
    }


    public override void Configure()
    {
        Post(ApiRoutes.Users.Login);
        AllowAnonymous();

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
        var tokens = await _jwtService.AccessAsync(user);

        await ApplicationDbContext.SaveChangesAsync(CancellationToken.None);
        await SendOkAsync(
            new()
            {
                Tokens = tokens
            },
            CancellationToken.None
        );
    }
}