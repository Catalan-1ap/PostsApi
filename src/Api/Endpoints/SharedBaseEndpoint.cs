using Core.Interfaces;
using FastEndpoints;


namespace Api.Endpoints;


public abstract class SharedBaseEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull, new()
    where TResponse : notnull, new()
{
    public abstract IApplicationDbContext ApplicationDbContext { get; init; }
}