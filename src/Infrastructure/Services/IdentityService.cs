using Application.Exceptions;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Services;


public sealed class IdentityService : IIdentityService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<User> _userManager;


    public IdentityService(UserManager<User> userManager, ICurrentUserService currentUserService)
    {
        _userManager = userManager;
        _currentUserService = currentUserService;
    }


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


    public async Task<IUser> Login(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
            throw new NotFoundException(nameof(User), email);

        var isPasswordAreValid = await _userManager.CheckPasswordAsync(user, password);

        if (isPasswordAreValid == false)
            throw new BusinessException("Email/Password combination is wrong");

        return user;
    }


    public async Task<IUser> CurrentUser()
    {
        var userId = _currentUserService.UserId;

        return await _userManager.FindByIdAsync(userId);
    }
}