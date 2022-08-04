using Api.Common;
using FastEndpoints.Swagger;


namespace Api.Installers;


public static class SwaggerInstaller
{
    public static void AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerDoc(x =>
            {
                x.Title = "PostsApi";
                x.Version = "v1";
                x.Description = "Simple API for managing posts";
            },
            serializerSettings: SharedOptions.SerializerOptions,
            tagIndex: 2,
            shortSchemaNames: true
        );
    }


    public static void UseSwagger(this WebApplication app)
    {
        app.UseOpenApi();
        app.UseSwaggerUi3(x =>
        {
            x.ConfigureDefaults();
            x.AdditionalSettings["tryItOutEnabled"] = false;
            x.Path = "";
        });
    }
}