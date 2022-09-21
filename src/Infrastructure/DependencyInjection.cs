using Core.Entities;
using Core.Interfaces;
using Infrastructure.Implementations;
using Infrastructure.Options;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure;


public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, PostgresOptions postgresOptions)
    {
        services.AddScoped<AuditableSaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>(PostgresOptionsFactory.Make(postgresOptions));
        services.AddIdentity<User, Role>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.RemoveIdentityValidation();

        services.AddSingleton<IDateTimeService, SystemDateTimeService>();
        services.AddScoped<IIdentityService, AspIdentityService>();
        services.AddScoped<IStaticFilesService, DefaultStaticFilesService>();
        services.AddScoped<IJwtService, DefaultJwtService>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }


    public static async Task InitializeInfrastructureAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // if you fall with exception here - visit (https://github.com/npgsql/npgsql/issues/3955) and install certificates
        await context.Database.MigrateAsync();
    }


    private static void RemoveIdentityValidation(this IServiceCollection services)
    {
        var userValidator = services.Single(descriptor => descriptor.ServiceType == typeof(IUserValidator<User>));
        var passwordValidator = services.Single(descriptor => descriptor.ServiceType == typeof(IPasswordValidator<User>));

        services.Remove(userValidator);
        services.Remove(passwordValidator);
    }
}