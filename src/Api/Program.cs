using System.Text.Json;
using Api.Filters;
using Api.Installers;
using Application;


var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers(x =>
    {
        x.Filters.Add<ExceptionFilter>();
    })
    .ConfigureApiBehaviorOptions(x =>
    {
        x.SuppressInferBindingSourcesForParameters = true;
        x.SuppressMapClientErrors = true;
    })
    .AddJsonOptions(x =>
    {
        x.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddApplication();

builder.Services.InstallSwaggerService();
builder.Services.InstallInfrastructureService();

var app = builder.Build();

app.InstallSwagger();
await app.InstallInfrastructure();

app.UseAuthorization();
app.MapControllers();

app.Run();