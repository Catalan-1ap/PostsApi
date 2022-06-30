using System;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;


namespace Api.IntegrationTests.Fixtures;


public class ApiFixture : DatabaseFixture
{
    public HttpClient Client { get; }


    public ApiFixture() =>
        Client = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(_ =>
            {
                Environment.SetEnvironmentVariable(Host.Key, Host.Value);
                Environment.SetEnvironmentVariable(User.Key, User.Value);
                Environment.SetEnvironmentVariable(Password.Key, Password.Value);
                Environment.SetEnvironmentVariable("POSTGRES_PORT", "5432");
            })
            .CreateClient();
}