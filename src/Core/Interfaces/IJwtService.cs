using Core.Entities;
using Core.Models;


namespace Core.Interfaces;


public interface IJwtService
{
    Task<JwtTokens> AccessAsync(User user);

    Task<JwtTokens> RefreshAsync(JwtTokens tokens);
}