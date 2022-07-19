using System.Security.Claims;
using Core.Interfaces;
using Infrastructure.Common;


namespace Api.Services;


public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;


    public string UserId => _httpContextAccessor.HttpContext?.User.FindFirstValue(Claims.Id)!;


    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;
}