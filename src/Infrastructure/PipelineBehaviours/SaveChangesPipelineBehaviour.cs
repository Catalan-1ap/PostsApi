using Infrastructure.Persistence;
using MediatR;


namespace Infrastructure.PipelineBehaviours;


internal sealed class SaveChangesPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ApplicationDbContext _dbContext;


    public SaveChangesPipelineBehaviour(ApplicationDbContext dbContext) => _dbContext = dbContext;


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