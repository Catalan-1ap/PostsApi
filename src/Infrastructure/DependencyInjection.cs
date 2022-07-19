using Core.Entities;
using Core.Interfaces;
using Infrastructure.Options;
using Infrastructure.Persistence;
using Infrastructure.PipelineBehaviours;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure;


public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, PostgresOptions postgresOptions)
    {
        services.AddDbContext<ApplicationDbContext>(PostgresOptionsFactory.Make(postgresOptions));
        services.AddIdentity<User, Role>(x =>
            {
                x.Password.RequireDigit = true;
                x.Password.RequireLowercase = false;
                x.Password.RequireUppercase = false;
                x.Password.RequiredUniqueChars = 0;
                x.Password.RequireNonAlphanumeric = false;
                x.Password.RequiredLength = 8;

                x.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddSingleton<IDateTimeService, DateTimeService>();
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(SaveChangesPipelineBehaviour<,>));
    }


    public static async Task InitializeInfrastructure(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // if you fall with exception here - visit (https://github.com/npgsql/npgsql/issues/3955) and install certificates
        await context.Database.MigrateAsync();
    }
}