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
        x.SuppressModelStateInvalidFilter = true;
        x.SuppressInferBindingSourcesForParameters = true;
        x.SuppressMapClientErrors = true;
    });
builder.Services.AddApplication();
builder.Services.InstallInfrastructureService();
builder.Services.InstallSwaggerService();

var app = builder.Build();

await app.InstallInfrastructure();
app.InstallSwagger();

app.UseAuthorization();
app.MapControllers();
app.Run();