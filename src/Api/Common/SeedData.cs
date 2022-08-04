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


    public async Task<bool> IsSeedingRequired()
    {
        bool seedingRequired = true;

        var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();

        if (await userManager.Users.AnyAsync())
            seedingRequired = false;

        return seedingRequired;
    }


    public async Task Seed()
    {
        var identity = _serviceProvider.GetRequiredService<IIdentityService>();
        var jwt = _serviceProvider.GetRequiredService<IJwtService>();

        var registerEndpoint = Factory.Create<RegisterEndpoint>(identity, jwt);

        await registerEndpoint.HandleAsync(new()
            {
                UserName = "admin",
                Email = "admin@example.com",
                Password = "VeryStrongPassword"
            },
            CancellationToken.None);
        await registerEndpoint.HandleAsync(new()
            {
                UserName = "notadmin",
                Email = "notadmin@example.com",
                Password = "VeryWeakPassword"
            },
            CancellationToken.None);
    }
}