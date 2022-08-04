using Api.Common;
using Api.Responses;
using Core.Interfaces;
using Core.Models;
using FastEndpoints;
using FluentValidation;


namespace Api.Endpoints.Auth;


public sealed class RefreshRequest
{
    public string RefreshToken { get; init; } = null!;
}


public sealed class RefreshResponse
{
    public JwtTokens Tokens { get; init; } = null!;
}


public sealed class RefreshValidator : Validator<RefreshRequest>
{
    public RefreshValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}


public sealed class RefreshEndpoint : Endpoint<RefreshRequest, RefreshResponse>
{
    private readonly IJwtService _jwtService;


    public RefreshEndpoint(IJwtService jwtService) => _jwtService = jwtService;


    public override void Configure()
    {
        Post(ApiRoutes.Auth.Refresh);
        AllowAnonymous();

        Summary(x =>
        {
            x.Response<RefreshResponse>(StatusCodes.Status200OK, "JWT tokens");
            x.Response<ValidationErrorResponse>(StatusCodes.Status400BadRequest, "Validation error");
            x.Response<SingleErrorResponse>(StatusCodes.Status400BadRequest, "Token has been expired");
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound, "Token does not exists");
        });
    }


    public override async Task HandleAsync(RefreshRequest req, CancellationToken ct)
    {
        var tokens = await _jwtService.Refresh(req.RefreshToken);

        await SendOkAsync(new()
            {
                Tokens = tokens
            },
            ct
        );
    }
}