using Core.Interfaces;
using FluentValidation;
using MediatR;


namespace Core.Features.Posts;


public sealed record UpdatePostRequest(Guid Id, string Title, string Body) : IRequest<UpdatePostResponse>;


public sealed record UpdatePostResponse(Guid Id, string Title, string Body);


public sealed class UpdatePostValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostValidator()
    {
        RuleFor(x => x.Id).ApplyIdRules();

        RuleFor(x => x.Title).ApplyTitleRules();

        RuleFor(x => x.Body).ApplyBodyRules();
    }
}


public sealed class UpdatePostHandler : IRequestHandler<UpdatePostRequest, UpdatePostResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;


    public UpdatePostHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }


    public async Task<UpdatePostResponse> Handle(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        await Prepare(request, cancellationToken);

        var post = await _dbContext.Posts
            .FindAsync(new object?[] { request.Id }, cancellationToken);

        post!.Title = request.Title;
        post.Body = request.Body;

        _dbContext.Posts.Update(post);

        return new(post.Id, post.Title, post.Body);
    }


    private async Task Prepare(UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var post = await ValidationRules.PostShouldExists(request.Id, _dbContext, cancellationToken);

        ValidationRules.CurrentUserShouldOwnPost(post, _currentUserService);
    }
}