using Application.PipelineBehaviours;
using FluentValidation;
using MediatR;
using MediatR.Extensions.AttributedBehaviors;
using Microsoft.Extensions.DependencyInjection;


namespace Application;


public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        ValidatorOptions.Global.LanguageManager.Enabled = false;

        services.AddMediatR(typeof(DependencyInjection).Assembly);

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehaviour<,>));

        services.AddMediatRAttributedBehaviors(typeof(DependencyInjection).Assembly);
    }
}