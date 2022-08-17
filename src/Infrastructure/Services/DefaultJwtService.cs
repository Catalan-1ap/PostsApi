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


public sealed class DefaultJwtService : IJwtService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IIdentityService _identityService;
    private readonly IJwtSettings _jwtSettings;


    public DefaultJwtService(
        IJwtSettings jwtSettings,
        IApplicationDbContext dbContext,
        IDateTimeService dateTimeService,
        IIdentityService identityService
    )
    {
        _jwtSettings = jwtSettings;
        _dateTimeService = dateTimeService;
        _identityService = identityService;
        _dbContext = dbContext;
    }


    public async Task<JwtTokens> AccessAsync(User user)
    {
        var claims = await CreateClaimsForUser(user);

        return CreateTokens(user.Id, claims);
    }


    public async Task<JwtTokens> RefreshAsync(JwtTokens tokens)
    {
        var principal = GetPrincipalFromToken(tokens.Access);
        var userId = principal.FindFirst(Claims.Id)!.Value;

        var tokenDetails = await _dbContext.RefreshTokens
            .SingleOrDefaultAsync(token => token.Token == tokens.Refresh && token.UserId == userId);

        if (tokenDetails is null)
            throw new NotFoundException();

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

        return new()
        {
            Access = accessToken,
            Refresh = refreshToken
        };
    }


    private string CreateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new(claims),
            Issuer = _jwtSettings.Issuer,
            SigningCredentials = _jwtSettings.Credentials,
            Expires = _jwtSettings.ExpiresForAccessToken,
            NotBefore = _dateTimeService.UtcNow
        };

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.CreateToken(tokenDescriptor);
        var accessToken = handler.WriteToken(jwtToken);

        return accessToken;
    }


    // TODO: Benchmark -> new/stackalloc/ArrayPool which faster
    private string CreateRefreshToken(string userId)
    {
        var bytes = new byte[128];
        Random.Shared.NextBytes(bytes);
        var refreshToken = Convert.ToBase64String(bytes);

        _dbContext.RefreshTokens.Add(
            new()
            {
                Token = refreshToken,
                UserId = userId,
                CreatedAt = _dateTimeService.UtcNowDate,
                ExpiredAt = DateOnly.FromDateTime(_jwtSettings.ExpiresForRefreshToken)
            }
        );

        return refreshToken;
    }


    private async Task<IEnumerable<Claim>> CreateClaimsForUser(User user)
    {
        var claims = new List<Claim>
        {
            new(Claims.Id, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var rolesAsClaims = (await _identityService.GetRolesAsync(user))
            .Select(userRole => new Claim(ClaimTypes.Role, userRole));
        claims.AddRange(rolesAsClaims);

        return claims;
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


    private bool IsTokenExpired(RefreshToken tokenDetails) =>
        DateOnly.FromDateTime(_dateTimeService.UtcNow) > tokenDetails.ExpiredAt;
}