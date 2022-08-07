using Api.Common;
using Api.Processors;
using Api.Responses;
using Api.Validators;
using Core.Interfaces;
using Core.Models;
using FastEndpoints;
using FluentValidation;


namespace Api.Endpoints.Auth;


public sealed class RefreshRequest
{
    public JwtTokens Tokens { get; init; } = null!;
}


public sealed class RefreshResponse
{
    public JwtTokens Tokens { get; init; } = null!;
}


public sealed class RefreshValidator : Validator<RefreshRequest>
{
    public RefreshValidator()
    {
        RuleFor(x => x.Tokens)
            .NotEmpty()
            .SetValidator(new JwtTokensValidator());
    }
}


public sealed class RefreshEndpoint : Endpoint<RefreshRequest, RefreshResponse>
{
    private readonly IJwtService _jwtService;


    public RefreshEndpoint(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }


    public override void Configure()
    {
        Post(ApiRoutes.Auth.Refresh);
        AllowAnonymous();
        PostProcessors(new SaveChangesPostProcessor<RefreshRequest, RefreshResponse>());

        Summary(x =>
        {
            x.Response<RefreshResponse>(StatusCodes.Status200OK, "JWT tokens");
            x.Response<SingleErrorResponse>(StatusCodes.Status400BadRequest, "Token has been expired");
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound, "Token does not exists");
        });
    }


    public override async Task HandleAsync(RefreshRequest req, CancellationToken ct)
    {
        var tokens = await _jwtService.RefreshAsync(req.Tokens);

        await SendOkAsync(new()
            {
                Tokens = tokens
            },
            ct
        );
    }
}