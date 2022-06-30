using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ductus.FluentDocker;
using Ductus.FluentDocker.Commands;


namespace Api.IntegrationTests.Fixtures;


public class DatabaseFixture : IDisposable
{
    protected static readonly KeyValuePair<string, string> Host = new("POSTGRES_HOST", "localhost");
    protected static readonly KeyValuePair<string, string> User = new("POSTGRES_USER", "sa");
    protected static readonly KeyValuePair<string, string> Password = new("POSTGRES_PASSWORD", "pass");
    private const string DockerComposeFileName = "docker-compose.yml";
    private const string DockerName = "IntegrationTest";
    private readonly Action _dockerDown;


    public DatabaseFixture()
    {
        var docker = Fd.Hosts().Native().Host;

        var composeFile = GetDockerComposeFileLocation();
        
        docker.ComposeUpCommand(new()
        {
            AltProjectName = DockerName,
            ComposeFiles = new List<string> { composeFile },
            Services = new List<string> { "database" },
            Env = new Dictionary<string, string>(new[]
            {
                User, Password
            })
        });
        _dockerDown = () => docker.ComposeDown(
            altProjectName: DockerName,
            removeVolumes: true,
            composeFile: composeFile);
    }


    public void Dispose()
    {
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