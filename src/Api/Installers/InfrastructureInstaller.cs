using Api.Common;
using Infrastructure;
using Infrastructure.Options;


namespace Api.Installers;


public static class InfrastructureInstaller
{
    public static void InstallInfrastructure(this IServiceCollection services)
    {
        services.AddInfrastructure(GetDbOptions());
    }


    public static async Task InitializeInfrastructure(this WebApplication app)
    {
        await app.Services.InitializeInfrastructure();
    }


    private static PostgresOptions GetDbOptions()
    {
        var envs = new[]
        {
            "POSTGRES_HOST",
            "POSTGRES_PORT",
            "POSTGRES_USER",
            "POSTGRES_PASSWORD"
        }.GetEnvironmentVariables();

        return new(
            envs["POSTGRES_HOST"],
            int.Parse(envs["POSTGRES_PORT"]),
            envs["POSTGRES_USER"],
            envs["POSTGRES_PASSWORD"]
        );
    }
}