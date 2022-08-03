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
    public GetPostByIdValidator()
    {
        RuleFor(x => x.Id).ApplyIdRules();
    }
}


public sealed class GetPostByIdHandler : IRequestHandler<GetPostByIdRequest, GetPostByIdResponse>
{
    private readonly IApplicationDbContext _dbContext;


    public GetPostByIdHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;


    public async Task<GetPostByIdResponse> Handle(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        await Prepare(request, cancellationToken);

        var post = await _dbContext.Posts
            .AsNoTracking()
            .Where(x => x.Id == request.Id)
            .Select(x => new GetPostByIdResponse(
                x.Id,
                x.Body,
                x.Title,
                new(x.Owner.Id, x.Owner.UserName)
            ))
            .SingleAsync(cancellationToken);

        return post;
    }


    private async Task Prepare(GetPostByIdRequest request, CancellationToken cancellationToken)
    {
        await ValidationRules.PostShouldExists(request.Id, _dbContext, cancellationToken);
    }
}