using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Application.Exceptions;
using Application.Interfaces;
using Application.Models;
using Application.Settings;
using Domain.NonDomainEntities;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Services;


public sealed class JwtService : IJwtService
{
    private const string IdClaimType = "id";
    private readonly IDateTimeService _dateTimeService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IJwtSettings _jwtOptions;


    public JwtService(IJwtSettings jwtOptions,
                      IApplicationDbContext dbContext,
                      IDateTimeService dateTimeService
    )
    {
        _jwtOptions = jwtOptions;
        _dateTimeService = dateTimeService;
        _dbContext = dbContext;
    }


    public JwtTokens Access(string userId)
    {
        var securityToken = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: CreateClaimsForUser(userId),
            signingCredentials: _jwtOptions.Credentials,
            expires: _jwtOptions.ExpiresForAccessToken,
            notBefore: _dateTimeService.UtcNow
        );

        var handler = new JwtSecurityTokenHandler();
        var accessToken = handler.WriteToken(securityToken);
        var refreshToken = CreateRefreshToken();
        _dbContext.RefreshTokens.Add(new(
            refreshToken,
            userId,
            DateOnly.FromDateTime(_jwtOptions.ExpiresForRefreshToken),
            _dateTimeService.UtcNowDate
        ));

        return new(accessToken, refreshToken);
    }


    public async Task<JwtTokens> Refresh(string refreshToken)
    {
        var tokenDetails = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.Token == refreshToken);

        if (tokenDetails is null)
            throw new NotFoundException(nameof(RefreshToken), refreshToken);

        _dbContext.RefreshTokens.Remove(tokenDetails);

        if (DateOnly.FromDateTime(_dateTimeService.UtcNow) > tokenDetails.ExpiredAt)
            throw new BusinessException("Token has been expired");

        var newTokens = Access(tokenDetails.UserId);

        return newTokens;
    }


    // TODO: Benchmark -> new or stackalloc
    private static string CreateRefreshToken()
    {
        var bytes = new byte[128];
        // Span<byte> bytes = stackalloc byte[128];
        Random.Shared.NextBytes(bytes);

        return Convert.ToBase64String(bytes);
    }


    private static IEnumerable<Claim> CreateClaimsForUser(string userId)
    {
        return new Claim[]
        {
            new(IdClaimType, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
    }
}