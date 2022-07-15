using Application.Interfaces;
using Application.Models;
using MediatR;


namespace Application.Features;


public sealed record RefreshRequest(string RefreshToken) : IRequest<RefreshResponse>;


public sealed record RefreshResponse(JwtTokens Tokens);


internal sealed class RefreshHandler : IRequestHandler<RefreshRequest, RefreshResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;


    public RefreshHandler(IIdentityService identityService, IJwtService jwtService)
    {
        _identityService = identityService;
        _jwtService = jwtService;
    }


    public async Task<RefreshResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokens = await _jwtService.Refresh(request.RefreshToken);

        return new(tokens);
    }
}