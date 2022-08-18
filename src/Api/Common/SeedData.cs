using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Api.Common;


public sealed class SeedData
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
        var roleManager = _serviceProvider.GetRequiredService<RoleManager<Role>>();

        if (await roleManager.RoleExistsAsync(Role.Admin) == false)
            await roleManager.CreateAsync(new() { Name = Role.Admin });

        var admin = await identity.RegisterAsync("admin", "admin@example.com", "VeryStrongPassword");
        await identity.AddToRoleAsync(admin, Role.Admin);

        await identity.RegisterAsync("notadmin", "notadmin@example.com", "VeryWeakPassword");
        await identity.RegisterAsync("notadmin1", "notadmin1@example.com", "VeryWeakPassword1");
    }
}