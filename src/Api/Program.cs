using Api.Common;
using Api.Installers;
using Api.Options;
using Api.Responses;
using Core.Models;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.FileProviders;


ValidatorOptions.Global.LanguageManager.Enabled = false;

StaticResourcesEnvironment BuildStartupEnvironment()
{
    var appRoot = Directory.GetCurrentDirectory();
    var contentRoot = Path.Combine(appRoot, "static");

    return new()
    {
        StaticRootFsPath = contentRoot,
        AvatarsFsPath = Path.Combine(
            contentRoot,
            "avatars"
        ),
        CoversFsPath = Path.Combine(
            contentRoot,
            "covers"
        )
    };
}

var staticResourcesEnvironment = BuildStartupEnvironment();
Directory.CreateDirectory(staticResourcesEnvironment.AvatarsFsPath);
Directory.CreateDirectory(staticResourcesEnvironment.CoversFsPath);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(staticResourcesEnvironment);
builder.Services.AddScoped(provider =>
{
    var request = provider.GetRequiredService<IHttpContextAccessor>().HttpContext!.Request;
    var baseUrl = $"{request?.Scheme}://{request?.Host.ToString()}/";

    return new CoreEnvironment
    {
        BaseUrl = new(baseUrl)
    };
});
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddFastEndpoints();
builder.Services.AddOptions<AvatarOptions>()
    .Bind(builder.Configuration.GetSection(AvatarOptions.Section))
    .ValidateDataAnnotations();
builder.Services.AddOptions<CoverImageOptions>()
    .Bind(builder.Configuration.GetSection(CoverImageOptions.Section))
    .ValidateDataAnnotations();

var app = builder.Build();

await app.UseInfrastructureAsync();
app.UseMiddleware<ExceptionMiddleware>();
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(staticResourcesEnvironment.StaticRootFsPath),
        RequestPath = string.Empty
    }
);
app.UseRouting();
app.UseCors(x => x.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(
    x =>
    {
        x.ShortEndpointNames = true;
        x.SerializerOptions = SharedOptions.SerializerOptions;
        x.GlobalEndpointOptions = (endpoint, builder) =>
        {
            if (endpoint.AnonymousVerbs is null)
            {
                builder.Produces(StatusCodes.Status401Unauthorized);
                endpoint.Summary?.Response(StatusCodes.Status401Unauthorized);
            }

            if (endpoint.ValidatorType is not null)
            {
                builder.Produces(StatusCodes.Status422UnprocessableEntity);
                endpoint.Summary?.Response<ValidationErrorResponse>(StatusCodes.Status422UnprocessableEntity, "Validation error");
            }
        };
        x.ErrorResponseBuilder = (failures, _) => failures.ToValidationErrorResponse();
        x.ErrorResponseStatusCode = StatusCodes.Status422UnprocessableEntity;
    }
);
app.UseSwagger();

using (var scope = app.Services.CreateScope())
{
    var seedData = new SeedData(scope.ServiceProvider);

    if (await seedData.IsSeedingRequiredAsync())
        await seedData.SeedAsync();
}

app.Run();