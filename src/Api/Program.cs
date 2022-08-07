using Api.Common;
using Api.Installers;
using Api.Responses;
using FastEndpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.TryAddSingleton<IActionResultExecutor<ObjectResult>, ObjectResultExecutor>();
builder.Configuration.AddEnvironmentVariables("API_");
builder.Services.AddFastEndpoints();
builder.Services.AddControllers();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddJwt(builder.Configuration);

var app = builder.Build();

await app.UseInfrastructureAsync();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(x =>
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
});
app.UseSwagger();

using (var scope = app.Services.CreateScope())
{
    var seedData = new SeedData(scope.ServiceProvider);

    if (await seedData.IsSeedingRequiredAsync())
        await seedData.SeedAsync();
}

app.Run();