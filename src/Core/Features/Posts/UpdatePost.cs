using Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Core.Features.Posts;


public sealed record UpdatePostRequest(Guid Id, string Title, string Body) : IRequest<UpdatePostResponse>;


public sealed record UpdatePostResponse(Guid Id, string Title, string Body);


public sealed class UpdatePostValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostValidator(IApplicationDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .MustAsync((id, cancellationToken) => ValidationRules.PostShouldExists(id, dbContext, cancellationToken));

        RuleFor(x => x.Title).ApplyTitleRules();

        RuleFor(x => x.Body).ApplyBodyRules();
    }
}


public sealed class UpdatePostHandler : IRequestHandler<UpdatePostRequest, UpdatePostResponse>
{
    private readonly IApplicationDbContext _dbContext;


    public UpdatePostHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;


    public async Task<UpdatePostResponse> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var post = await _dbContext.Posts
            .SingleAsync(post => post.Id == request.Id, cancellationToken);

        post.Title = request.Title;
        post.Body = request.Body;

        _dbContext.Posts.Update(post);

        return new(post.Id, post.Title, post.Body);
    }
}