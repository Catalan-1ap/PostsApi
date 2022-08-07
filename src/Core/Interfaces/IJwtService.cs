using Core.Entities;
using Core.Models;


namespace Core.Interfaces;


public interface IJwtService
{
    JwtTokens Access(User user);

    Task<JwtTokens> RefreshAsync(JwtTokens tokens);
}