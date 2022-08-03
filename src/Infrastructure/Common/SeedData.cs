using Core.Entities;
using Core.Features.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure.Common;


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
        var mediator = _serviceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new RegisterRequest("admin", "admin@example.com", "VeryStrongPassword"));
        await mediator.Send(new RegisterRequest("notadmin", "notadmin@example.com", "VeryWeakPassword"));
    }
}