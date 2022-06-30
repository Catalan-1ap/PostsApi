using System.Net.Http.Json;
using System.Threading.Tasks;
using Api.IntegrationTests.Fixtures;
using Application.Features.CreatePost;
using Domain;
using FluentAssertions;
using Xunit;


namespace Api.IntegrationTests.Tests;


[Collection(nameof(ApiCollection))]
public class PostControllerTests
{
    private readonly ApiFixture _apiFixture;


    public PostControllerTests(ApiFixture apiFixture) => _apiFixture = apiFixture;


    [Fact]
    public async Task Create_ShouldWork_WhenAllOk()
    {
        // Arrange
        var request = new CreatePostCommand.CreatePost("Title", "Body");

        // Act
        var response = await _apiFixture.Client.PostAsJsonAsync("api/posts", request);

        var post = await response.Content.ReadFromJsonAsync<Post>();

        // Assert
        post.Should().NotBeNull();
        post!.Id.Should().NotBeEmpty();
        post.Title.Should().BeEquivalentTo(request.Title);
        post.Body.Should().BeEquivalentTo(request.Body);
    }
}