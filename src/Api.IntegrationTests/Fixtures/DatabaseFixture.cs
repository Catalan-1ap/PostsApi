using System;
using System.Collections.Generic;
using System.Linq;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;


namespace Api.IntegrationTests.Fixtures;


public class DatabaseFixture : IDisposable
{
    protected static readonly KeyValuePair<string, string> Host = new("POSTGRES_HOST", "localhost");
    protected static readonly KeyValuePair<string, string> User = new("POSTGRES_USER", "sa");
    protected static readonly KeyValuePair<string, string> Password = new("POSTGRES_PASSWORD", "pass");
    private readonly IContainerService _container;


    public DatabaseFixture() =>
        _container = new Builder()
            .UseContainer()
            .UseImage("postgres:alpine")
            .ExposePort(5432, 5432)
            .WithEnvironment(ConvertKVPToEnvPairs(Host, User, Password))
            .UseHealthCheck($"\"pg_isready -U {User.Value}\"", "10s", "5s", retries: 5)
            .WaitForHealthy()
            .Build()
            .Start();


    public void Dispose()
    {
        _container.Dispose();
    }


    private static string[] ConvertKVPToEnvPairs(params KeyValuePair<string, string>[] pairs) =>
        pairs.Select(pair => $"{pair.Key}={pair.Value}").ToArray();
}