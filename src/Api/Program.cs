using Api.Common;
using Api.Installers;
using Api.Options;
using Api.Responses;
using FastEndpoints;
using FluentValidation;
using Infrastructure.Common;
using Microsoft.Extensions.FileProviders;


ValidatorOptions.Global.LanguageManager.Enabled = false;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddFastEndpoints();
builder.Services.AddOptions<AvatarOptions>()
    .Bind(builder.Configuration.GetSection(AvatarOptions.Section))
    .ValidateDataAnnotations();

var app = builder.Build();

await app.UseInfrastructureAsync();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
Directory.CreateDirectory(builder.Environment.FsAvatarsPath());
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider = new PhysicalFileProvider(builder.Environment.FsAvatarsPath()),
        RequestPath = ApiRoutes.AvatarsStatic.RequestPath
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