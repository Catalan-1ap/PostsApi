using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Services;


public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;


    public IdentityService(UserManager<User> userManager)
    {
        _userManager = userManager;
    }


    public async Task<User> RegisterAsync(string userName, string email, string password)
    {
        var id = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = id,
            UserName = userName,
            Email = email
        };

        var result = await _userManager.CreateAsync(user, password);
        
        if (result.Errors.Any())
            throw new SeveralErrorsException(result.Errors.Select(x => x.Description));

        return user;
    }


    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw NotFoundException.Make(nameof(User), email);

        var isPasswordAreValid = await _userManager.CheckPasswordAsync(user, password);

        if (isPasswordAreValid == false)
            throw new BusinessException("Email/Password combination is wrong");

        return user;
    }


    public async Task<User> GetByIdAsync(string id) => await _userManager.FindByIdAsync(id);


    public async Task<bool> IsUsernameUniqueAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);

        return user is null;
    }


    public async Task<bool> IsEmailUniqueAsync(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);

        return user is null;
    }
}