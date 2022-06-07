using Application.Interfaces;
using MediatR;


namespace Application.PipelineBehaviours;


public sealed class SaveChangesPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IApplicationDbContext _dbContext;


    public SaveChangesPipelineBehaviour(IApplicationDbContext dbContext) => _dbContext = dbContext;


    public async Task<TResponse> Handle(
        TRequest request,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var response = await next();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return response;
    }
}