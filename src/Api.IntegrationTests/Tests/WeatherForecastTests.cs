using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;


namespace Api.IntegrationTests.Tests;


[Collection(nameof(ApiCollection))]
public class WeatherForecastTests
{
    private HttpClient _api;
    
    public WeatherForecastTests(ApiFixture api)
    {
        _api = api.Client;
    }
    
    [Fact]
    public async Task ShouldReturnCorrectSummaries()
    {
        var response = await _api.GetAsync("WeatherForecast/summaries");
        
        response.EnsureSuccessStatusCode();
        Assert.Equal(new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        }, await response.Content.ReadFromJsonAsync<string[]>());
    }
}