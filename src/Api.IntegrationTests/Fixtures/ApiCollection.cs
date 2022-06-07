using Xunit;


namespace Api.IntegrationTests.Fixtures;


[CollectionDefinition(nameof(ApiCollection))]
public class ApiCollection : ICollectionFixture<ApiFixture> { }