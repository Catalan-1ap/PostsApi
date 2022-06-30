using Application.Interfaces;
using Infrastructure.Options;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure;


public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, DbOptions dbOptions)
    {
        services.AddDbContext<ApplicationDbContext>(PostgresOptionsFactory.Make(dbOptions));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
    }


    public static async Task InitializeInfrastructure(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // if you fall with exception here - visit (https://github.com/npgsql/npgsql/issues/3955) and install certiicates
        await context.Database.MigrateAsync();
    }
}