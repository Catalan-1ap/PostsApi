using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using MediatR;


namespace Core.Features.Posts;


public sealed record CreatePostRequest(string Title, string Body) : IRequest<CreatePostResponse>;


public sealed record CreatePostResponse(Guid Id, string Title, string Body);


public sealed class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title).ApplyTitleRules();

        RuleFor(x => x.Body).ApplyBodyRules();
    }
}


public sealed class CreatePostHandler : IRequestHandler<CreatePostRequest, CreatePostResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;


    public CreatePostHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }


    public Task<CreatePostResponse> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var (title, body) = request;
        var currentUser = _currentUserService.UserId;

        var newPost = new Post
        {
            Title = title,
            Body = body,
            OwnerId = currentUser
        };

        _dbContext.Posts.Add(newPost);

        var response = new CreatePostResponse(newPost.Id, newPost.Title, newPost.Body);

        return Task.FromResult(response);
    }
}