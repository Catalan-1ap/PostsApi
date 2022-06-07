using System.Reflection;


namespace Api.Installers;


public static class SwaggerInstaller
{
    public static void InstallSwaggerService(this IServiceCollection services)
    {
        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new() { Title = "PostsApi", Version = "v1" });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            x.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }


    public static void InstallSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "PostsApi");
            x.RoutePrefix = string.Empty;
        });
    }
}