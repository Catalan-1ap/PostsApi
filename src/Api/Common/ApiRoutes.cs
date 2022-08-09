﻿namespace Api.Common;


public static class ApiRoutes
{
    private const string Root = "api";


    public static class Posts
    {
        private const string Base = "posts";

        public const string GetById = $"{Root}/{Base}/{{id:guid}}";
        public const string Create = $"{Root}/{Base}";
        public const string Update = $"{Root}/{Base}/{{id:guid}}";
        public const string Delete = $"{Root}/{Base}/{{id:guid}}";
    }


    public static class Auth
    {
        private const string Base = "auth";

        public const string Register = $"{Root}/{Base}/register";
        public const string Login = $"{Root}/{Base}/login";
        public const string Refresh = $"{Root}/{Base}/refresh";
    }
}