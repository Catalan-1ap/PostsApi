using Core.Interfaces;
using Core.Models;
using MediatR;


namespace Core.Features.Auth;


public sealed record RefreshRequest(string RefreshToken) : IRequest<RefreshResponse>;


public sealed record RefreshResponse(JwtTokens Tokens);


public sealed class RefreshHandler : IRequestHandler<RefreshRequest, RefreshResponse>
{
    private readonly IJwtService _jwtService;


    public RefreshHandler(IJwtService jwtService) => _jwtService = jwtService;


    public async Task<RefreshResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokens = await _jwtService.Refresh(request.RefreshToken);

        return new(tokens);
    }
}