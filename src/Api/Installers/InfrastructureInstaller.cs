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
        var envs = new Dictionary<string, string?>
        {
            ["POSTGRES_HOST"] = null,
            ["POSTGRES_PORT"] = null,
            ["POSTGRES_USER"] = null,
            ["POSTGRES_PASSWORD"] = null
        };
        foreach (var envsKey in envs.Keys)
            envs[envsKey] = Environment.GetEnvironmentVariable(envsKey);

        var notDefinedEnvs = envs.Where(pair => pair.Value is null).ToArray();
        if (notDefinedEnvs.Any())
        {
            var errors = notDefinedEnvs
                .Select(x => $"Environment variable \"{x.Key}\" isn't defined");
            var errorDescription = string.Join(Environment.NewLine, errors);

            throw new(errorDescription);
        }

        return new(
            envs["POSTGRES_HOST"]!,
            int.Parse(envs["POSTGRES_PORT"]!),
            envs["POSTGRES_USER"]!,
            envs["POSTGRES_PASSWORD"]!
        );
    }
}