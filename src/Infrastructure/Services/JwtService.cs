using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using Core.Settings;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


namespace Infrastructure.Services;


public sealed class JwtService : IJwtService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IJwtSettings _jwtSettings;


    public JwtService(
        IJwtSettings jwtSettings,
        IApplicationDbContext dbContext,
        IDateTimeService dateTimeService
    )
    {
        _jwtSettings = jwtSettings;
        _dateTimeService = dateTimeService;
        _dbContext = dbContext;
    }


    public JwtTokens Access(User user) => CreateTokens(user.Id, CreateClaimsForUser(user));


    public async Task<JwtTokens> RefreshAsync(JwtTokens tokens)
    {
        var (access, refresh) = tokens;
        var principal = GetPrincipalFromToken(access);
        var userId = principal.FindFirst(Claims.Id)!.Value;

        var tokenDetails = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.Token == refresh && token.UserId == userId);

        if (tokenDetails is null)
            throw NotFoundException.Make(nameof(RefreshToken), refresh);

        _dbContext.RefreshTokens.Remove(tokenDetails);

        if (IsTokenExpired(tokenDetails))
        {
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            throw new BusinessException("Token has been expired");
        }

        var newTokens = CreateTokens(userId, principal.Claims);

        return newTokens;
    }


    private JwtTokens CreateTokens(string userId, IEnumerable<Claim> claims)
    {
        var accessToken = CreateAccessToken(claims);
        var refreshToken = CreateRefreshToken(userId);

        return new(accessToken, refreshToken);
    }

    
    private string CreateAccessToken(IEnumerable<Claim> claims)
    {
        var securityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            claims: claims,
            signingCredentials: _jwtSettings.Credentials,
            expires: _jwtSettings.ExpiresForAccessToken,
            notBefore: _dateTimeService.UtcNow
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(securityToken);

        return accessToken;
    }


    // TODO: Benchmark -> new/stackalloc/ArrayPool which faster
    private string CreateRefreshToken(string userId)
    {
        var bytes = new byte[128];
        Random.Shared.NextBytes(bytes);
        var refreshToken = Convert.ToBase64String(bytes);
        
        _dbContext.RefreshTokens.Add(new()
        {
            Token = refreshToken,
            UserId = userId,
            CreatedAt = _dateTimeService.UtcNowDate,
            ExpiredAt = DateOnly.FromDateTime(_jwtSettings.ExpiresForRefreshToken)
        });

        return refreshToken;
    }


    private static IEnumerable<Claim> CreateClaimsForUser(User user)
    {
        return new Claim[]
        {
            new(Claims.Id, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
    }
    

    private ClaimsPrincipal GetPrincipalFromToken(string accessToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal;
        SecurityToken validatedToken;
        
        try
        {
            principal = tokenHandler.ValidateToken(accessToken, _jwtSettings.TokenValidationParameters, out validatedToken);
        }
        catch
        {
            throw new BusinessException("Invalid token");
        }
        
        if (IsJwtTokenHasValidAlghorithm(validatedToken) == false)
            throw new BusinessException("Invalid token");

        return principal;
    }


    private bool IsJwtTokenHasValidAlghorithm(SecurityToken token) =>
        token is JwtSecurityToken jwtSecurityToken &&
        jwtSecurityToken.Header.Alg.Equals(_jwtSettings.Credentials.Algorithm, StringComparison.OrdinalIgnoreCase);
    
    
    private bool IsTokenExpired(RefreshToken tokenDetails) => DateOnly.FromDateTime(_dateTimeService.UtcNow) > tokenDetails.ExpiredAt;
}