using Api.Common;
using Api.Installers;
using Api.Services;
using Core.Interfaces;
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
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

await app.UseInfrastructure();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(x =>
{
    x.ShortEndpointNames = true;
    x.SerializerOptions = SharedOptions.SerializerOptions;
    x.ErrorResponseBuilder = (failures, _) => failures.ToValidationErrorResponse();
});
app.UseSwagger();

using (var scope = app.Services.CreateScope())
{
    var seedData = new SeedData(scope.ServiceProvider);

    if (await seedData.IsSeedingRequired())
        await seedData.Seed();
}

app.Run();