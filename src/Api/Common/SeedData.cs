using Api.Endpoints.Auth;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Api.Common;


public class SeedData
{
    private readonly IServiceProvider _serviceProvider;


    public SeedData(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;


    public async Task<bool> IsSeedingRequiredAsync()
    {
        bool seedingRequired = true;

        var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

        if (await userManager.Users.AnyAsync())
            seedingRequired = false;

        return seedingRequired;
    }


    public async Task SeedAsync()
    {
        var identity = _serviceProvider.GetRequiredService<IIdentityService>();

        await identity.RegisterAsync("admin", "admin@example.com", "VeryStrongPassword");
        await identity.RegisterAsync("notadmin", "notadmin@example.com", "VeryWeakPassword");
    }
}