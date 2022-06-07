namespace Api.Common;


public static class Routes
{
    public const string Root = "api";


    public static class Posts
    {
        private const string Base = "posts";

        public const string GetById = $"{Root}/{Base}/{{id:guid}}";
        public const string Create = $"{Root}/{Base}";
    }
}


public sealed record ValidationError(Dictionary<string, string[]> Errors);