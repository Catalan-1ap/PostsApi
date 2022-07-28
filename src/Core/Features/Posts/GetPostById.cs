using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Core.Features.Posts;


public sealed record GetPostByIdRequest(Guid Id) : IRequest<GetPostByIdResponse>;


public sealed record GetPostByIdResponse(Guid Id, string Title, string Body, GetPostByIdResponse.OwnerInfo Owner)
{
    public sealed record OwnerInfo(string Id, string UserName);
}


public sealed class GetPostByIdValidator : AbstractValidator<GetPostByIdRequest>
{
    public GetPostByIdValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync((id, cancellationToken) => ValidationRules.PostShouldExists(id, dbContext, cancellationToken));
    }
}


public sealed class GetPostByIdHandler : IRequestHandler<GetPostByIdRequest, GetPostByIdResponse>
{
    private readonly IApplicationDbContext _dbContext;


    public GetPostByIdHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;


    public async Task<GetPostByIdResponse> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetPostByIdResponse(
                x.Id,
                x.Body,
                x.Title,
                new(x.Owner.Id, x.Owner.UserName)
            ))
            .SingleOrDefaultAsync(cancellationToken);

        if (post is null)
            throw NotFoundException.Make(nameof(Post), request.Id);

        return post;
    }
}