using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Ductus.FluentDocker;
using Ductus.FluentDocker.Commands;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


namespace Api.IntegrationTests;


[CollectionDefinition(nameof(ApiCollection))]
public class ApiCollection : ICollectionFixture<ApiFixture> { }


public class ApiFixture : DatabaseFixture
{
    public HttpClient Client { get; }


    public ApiFixture() =>
        Client = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ =>
            {
                Environment.SetEnvironmentVariable("POSTGRES_USER", "sa");
                Environment.SetEnvironmentVariable("POSTGRES_PASSWORD", "pass");
                Environment.SetEnvironmentVariable("POSTGRES_PORT", "8001");
            })
            .CreateClient();
}


public class DatabaseFixture : IDisposable
{
    private const string DockerComposeFileName = "docker-compose.yml";
    private readonly Action _dockerDown;


    public DatabaseFixture()
    {
        var docker = Fd.Hosts().Native().Host;

        var composeFile = GetDockerComposeFileLocation();
        
        docker.ComposeUpCommand(new()
        {
            ComposeFiles = new List<string> { composeFile },
            Services = new List<string> { "database" }
        });
        _dockerDown = () => docker.ComposeDown(removeVolumes: true, composeFile: composeFile);
    }


    public void Dispose()
    {
        GC.SuppressFinalize(this);

        _dockerDown();
    }


    private string GetDockerComposeFileLocation()
    {
        var compose = (string?)null;

        for (var current = new DirectoryInfo(Directory.GetCurrentDirectory());
             compose is null;
             current = current.Parent!)
        {
            var isComposeFolder = current
                .EnumerateFiles()
                .Any(fileInfo => fileInfo.Name.Equals(DockerComposeFileName, StringComparison.Ordinal));

            if (isComposeFolder)
                compose = Path.Combine(current!.FullName, DockerComposeFileName);
        }

        return compose;
    }
}