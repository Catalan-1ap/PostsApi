using System.Reflection;
using FluentValidation;
using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;


namespace Api.Installers;


public static class SwaggerInstaller
{
    public static void InstallSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1",
                new()
                {
                    Title = "PostsApi",
                    Version = "v1",
                    Description = "Simple API for managing posts"
                });

            x.AddSecurityDefinition("Bearer",
                new()
                {
                    In = ParameterLocation.Header,
                    Description = "JSON Web Token to access resources. Example: Bearer {token}",
                    Scheme = "Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

            x.AddSecurityRequirement(new()
            {
                {
                    new()
                    {
                        Reference = new() { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new[] { string.Empty }
                }
            });

            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            x.IncludeXmlComments(
                Path.Combine(AppContext.BaseDirectory, xmlFilename),
                includeControllerXmlComments: true
            );
        });
        services.TryAddTransient<IValidatorFactory, ServiceProviderValidatorFactory>();
        services.AddFluentValidationRulesToSwagger();
    }


    public static void InitializeSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(x =>
        {
            x.SwaggerEndpoint("/swagger/v1/swagger.json", "PostsApi");
            x.RoutePrefix = string.Empty;
        });
    }
}