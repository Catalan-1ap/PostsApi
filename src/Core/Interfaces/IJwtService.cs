using Core.Models;


namespace Core.Interfaces;


public interface IJwtService
{
    JwtTokens Access(string userId);

    Task<JwtTokens> Refresh(string refreshToken);
}