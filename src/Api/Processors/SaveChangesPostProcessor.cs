using Core.Interfaces;
using FastEndpoints;
using FluentValidation.Results;


namespace Api.Processors;


public sealed class SaveChangesPostProcessor<TRequest, TResponse> : IPostProcessor<TRequest, TResponse>
{
    public async Task PostProcessAsync(
        TRequest req,
        TResponse res,
        HttpContext ctx,
        IReadOnlyCollection<ValidationFailure> failures,
        CancellationToken ct
    )
    {
        var dbContext = ctx.RequestServices.GetRequiredService<IApplicationDbContext>();

        await dbContext.SaveChangesAsync(ct);
    }
}