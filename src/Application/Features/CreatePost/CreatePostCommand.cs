using Application.Interfaces;
using Application.PipelineBehaviours;
using Application.StorageContracts;
using Domain;
using FluentValidation;
using MediatR;
using MediatR.Extensions.AttributedBehaviors;


namespace Application.Features.CreatePost;


public static class CreatePostCommand
{
    [MediatRBehavior(typeof(SaveChangesPipelineBehaviour<CreatePost, Post>))]
    public sealed record CreatePost(string Title, string Body) : IRequest<Post>;


    public sealed class Validator : AbstractValidator<CreatePost>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Is required")
                .MaximumLengthWithMessage(PostStorageContract.TitleMaxLength);

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Is required")
                .MaximumLengthWithMessage(PostStorageContract.BodyMaxLength);
        }
    }


    internal sealed class Handler : IRequestHandler<CreatePost, Post>
    {
        private readonly IApplicationDbContext _dbContext;


        public Handler(IApplicationDbContext dbContext) => _dbContext = dbContext;


        public Task<Post> Handle(CreatePost request, CancellationToken cancellationToken)
        {
            var (title, body) = request;
            var newPost = new Post(title, body);

            _dbContext.Posts.Add(newPost);

            return Task.FromResult(newPost);
        }
    }
}