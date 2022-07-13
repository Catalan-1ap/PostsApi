using Application.Interfaces;
using Application.PipelineBehaviours;
using Application.StorageContracts;
using Domain;
using FluentValidation;
using MediatR;
using MediatR.Extensions.AttributedBehaviors;


namespace Application.Features;


[MediatRBehavior(typeof(SaveChangesPipelineBehaviour<CreatePostRequest, Post>))]
public sealed record CreatePostRequest(string Title, string Body) : IRequest<Post>;


public sealed class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLengthWithMessage(PostStorageContract.TitleMaxLength);

        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLengthWithMessage(PostStorageContract.BodyMaxLength);
    }
}


internal sealed class CreatePostHandler : IRequestHandler<CreatePostRequest, Post>
{
    private readonly IApplicationDbContext _dbContext;


    public CreatePostHandler(IApplicationDbContext dbContext) => _dbContext = dbContext;


    public Task<Post> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var (title, body) = request;
        var newPost = new Post { Title = title, Body = body };

        _dbContext.Posts.Add(newPost);

        return Task.FromResult(newPost);
    }
}