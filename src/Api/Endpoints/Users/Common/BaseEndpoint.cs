using Api.Common;


namespace Api.Endpoints.Users.Common;


public abstract class BaseEndpoint<TRequest, TResponse> : SharedBaseEndpoint<TRequest, TResponse>
    where TRequest : notnull, new()
    where TResponse : notnull, new()
{
    protected string CreateAvatarUri(string fileName)
    {
        var baseUri = new Uri(BaseURL);
        var avatarUri = new Uri(baseUri, $"{ApiRoutes.AvatarsStatic.RequestPath}/{fileName}");

        return avatarUri.ToString();
    }
}