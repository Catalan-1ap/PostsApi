using Infrastructure;
using Infrastructure.Options;


namespace Api.Installers;


public static class InfrastructureInstaller
{
    public static void InstallInfrastructureService(this IServiceCollection services)
    {
        services.AddInfrastructure(GetDbOptions());
    }


    public static async Task InstallInfrastructure(this WebApplication app)
    {
        await app.Services.InitializeInfrastructure();
    }


    private static DbOptions GetDbOptions()
    {
        var host = Environment.GetEnvironmentVariable("POSTGRES_HOST");
        var port = Environment.GetEnvironmentVariable("POSTGRES_PORT");
        var user = Environment.GetEnvironmentVariable("POSTGRES_USER");
        var pass = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");

        var notDefinedEnvs = new[]
        {
            host, port, user, pass
        }.Where(x => x is null).ToArray();

        if (notDefinedEnvs.Any())
        {
            var errors = notDefinedEnvs
                .Select(x => $"Environment variable \"{x}\" isn't defined");
            var errorDescription = string.Join(Environment.NewLine, errors);

            throw new(errorDescription);
        }

        return new(
            host!,
            int.Parse(port!),
            user!,
            pass!
        );
    }
}