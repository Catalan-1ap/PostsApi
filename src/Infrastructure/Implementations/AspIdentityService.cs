using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Common;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Implementations;


public sealed class AspIdentityService : IIdentityService
{
    private readonly UserManager<User> _userManager;


    public AspIdentityService(UserManager<User> userManager)
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
            throw result.ToSeveralErrorsException();

        return user;
    }


    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new BusinessException("Email/Password combination is wrong");

        var isPasswordAreValid = await _userManager.CheckPasswordAsync(user, password);

        if (isPasswordAreValid == false)
            throw new BusinessException("Email/Password combination is wrong");

        return user;
    }


    public async Task AddToRoleAsync(User user, string role)
    {
        var result = await _userManager.AddToRoleAsync(user, role);

        if (result.Errors.Any())
            throw result.ToSeveralErrorsException();
    }


    public async Task<IList<string>> GetRolesAsync(User user) => await _userManager.GetRolesAsync(user);

    public async Task<bool> IsInRoleAsync(User user, string role) => await _userManager.IsInRoleAsync(user, role);


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