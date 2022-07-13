using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Services;


public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;


    public IdentityService(UserManager<User> userManager) => _userManager = userManager;


    public async Task<string> Register(string userName, string email, string password)
    {
        var id = Guid.NewGuid().ToString();
        var result = await _userManager.CreateAsync(new()
            {
                Id = id,
                Email = email,
                UserName = userName
            },
            password);

        if (result.Errors.Any())
            throw new SeveralErrorsException(result.Errors.Select(x => x.Description));

        return id;
    }
}