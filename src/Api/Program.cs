using System.Text.Json;
using Api.Common;
using Api.Installers;
using Api.Services;
using Application;
using Application.Interfaces;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .ConfigureApiBehaviorOptions(x =>
    {
        x.SuppressModelStateInvalidFilter = true;
        x.SuppressInferBindingSourcesForParameters = true;
        x.SuppressMapClientErrors = true;
    })
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddApplication();
builder.Services.InstallInfrastructure();
builder.Services.InstallSwagger();
builder.Services.InstallJwt(builder.Configuration);
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

var app = builder.Build();

await app.InitializeInfrastructure();
app.InitializeSwagger();

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(x =>
{
    x.MapControllers();
});
app.Run();