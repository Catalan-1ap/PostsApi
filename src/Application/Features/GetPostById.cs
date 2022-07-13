using Application.Exceptions;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Features;


public sealed record GetPostByIdRequest(Guid Id) : IRequest<Post>;


internal sealed class GetPostByIdHandler : IRequestHandler<GetPostByIdRequest, Post>
{
    private readonly IApplicationDbContext _dbContext;


    public GetPostByIdHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;


    public async Task<Post> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts.SingleOrDefaultAsync(
            x => x.Id.Equals(request.Id),
            cancellationToken
        );

        if (post is null)
            throw new NotFoundException(nameof(Post), request.Id);

        return post;
    }
}