using Application.Interfaces;
using Application.Models;
using MediatR;


namespace Application.Features;


public sealed record RefreshRequest(string RefreshToken) : IRequest<RefreshResponse>;


public sealed record RefreshResponse(JwtTokens Tokens);


internal sealed class RefreshHandler : IRequestHandler<RefreshRequest, RefreshResponse>
{
    private readonly IJwtService _jwtService;


    public RefreshHandler(IJwtService jwtService) => _jwtService = jwtService;


    public async Task<RefreshResponse> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var tokens = await _jwtService.Refresh(request.RefreshToken);

        return new(tokens);
    }
}