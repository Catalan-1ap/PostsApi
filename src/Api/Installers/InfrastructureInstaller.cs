﻿using Api.Common;
using Infrastructure;
using Infrastructure.Options;


namespace Api.Installers;


public static class InfrastructureInstaller
{
    public static void InstallInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        var section = configuration.GetSection(PostgresOptions.Section);
        var options = section.Get<PostgresOptions>();

        options.ValidateDataAnnotations();
        services.Configure<PostgresOptions>(section);

        services.AddInfrastructure(options);
    }


    public static async Task InitializeInfrastructure(this WebApplication app)
    {
        await app.Services.InitializeInfrastructure();
    }
}