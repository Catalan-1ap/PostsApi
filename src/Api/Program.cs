using Api.Common;
using Api.Installers;
using Api.Options;
using Api.Responses;
using Core;
using Core.Models;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.FileProviders;


ValidatorOptions.Global.LanguageManager.Enabled = false;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(x =>
{
    var baseUrl = builder.Configuration[WebHostDefaults.ServerUrlsKey];
    var appRoot = x.GetRequiredService<IHostEnvironment>().ContentRootPath;
    var contentRoot = Path.Combine(appRoot, "StaticFiles");

    return new CoreEnvironment
    {
        BaseUri = new(baseUrl),
        StaticRootFsPath = contentRoot,
        AvatarsFsPath = Path.Combine(
            contentRoot,
            "Avatars"
        ),
        CoversFsPath = Path.Combine(
            contentRoot,
            "Covers"
        )
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
var coreEnvironment = app.Services.GetRequiredService<CoreEnvironment>();

await app.UseInfrastructureAsync();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
Directory.CreateDirectory(coreEnvironment.AvatarsFsPath);
Directory.CreateDirectory(coreEnvironment.CoversFsPath);
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(coreEnvironment.AvatarsFsPath),
        RequestPath = ApiRoutes.Static.Avatars
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