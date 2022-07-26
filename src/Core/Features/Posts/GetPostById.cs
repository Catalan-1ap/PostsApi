using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Core.Features.Posts;


public sealed record GetPostByIdRequest(Guid Id) : IRequest<GetPostByIdResponse>;


public sealed record GetPostByIdResponse(Guid Id, string Title, string Body, GetPostByIdResponse.OwnerInfo Owner)
{
    public sealed record OwnerInfo(string Id, string UserName);
}


internal sealed class GetPostByIdHandler : IRequestHandler<GetPostByIdRequest, GetPostByIdResponse>
{
    private readonly IApplicationDbContext _dbContext;


    public GetPostByIdHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;


    public async Task<GetPostByIdResponse> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .Where(x => x.Id == request.Id)
            .Select(x => new GetPostByIdResponse(
                x.Id,
                x.Body,
                x.Title,
                new(x.Owner.Id, x.Owner.UserName)
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (post is null)
            throw new NotFoundException(nameof(Post), request.Id);

        return post;
    }
}