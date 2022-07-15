using Application.Models;


namespace Application.Interfaces;


public interface IJwtService
{
    JwtTokens Access(string userId);

    Task<JwtTokens> Refresh(string refreshToken);
}