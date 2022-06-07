using Application.Exceptions;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Application.Features.GetById;


public static class GetByIdQuery
{
    public sealed record GetById(Guid Id) : IRequest<Post>;


    internal sealed class Handler : IRequestHandler<GetById, Post>
    {
        private readonly IApplicationDbContext _dbContext;


        public Handler(IApplicationDbContext dbContext) => _dbContext = dbContext;


        public async Task<Post> Handle(GetById request, CancellationToken cancellationToken)
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
}