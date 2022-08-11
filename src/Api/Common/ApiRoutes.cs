namespace Api.Common;


public static class ApiRoutes
{
    private const string Root = "api";


    public static class Posts
    {
        private const string Base = "posts";

        public const string GetById = $"{Root}/{Base}/{{postId:guid}}";
        public const string Create = $"{Root}/{Base}";
        public const string Update = $"{Root}/{Base}/{{postId:guid}}";
        public const string Delete = $"{Root}/{Base}/{{postId:guid}}";
        public const string Like = $"{Root}/{Base}/{{postId:guid}}/like";
        public const string Dislike = $"{Root}/{Base}/{{postId:guid}}/dislike";
    }


    public static class Auth
    {
        private const string Base = "auth";

        public const string Register = $"{Root}/{Base}/register";
        public const string Login = $"{Root}/{Base}/login";
        public const string Refresh = $"{Root}/{Base}/refresh";
    }
}