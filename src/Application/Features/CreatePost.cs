﻿using Application.Interfaces;
using Application.StorageContracts;
using Domain;
using FluentValidation;
using MediatR;


namespace Application.Features;


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


    public async Task<Post> Handle(CreatePostRequest request, CancellationToken cancellationToken)
    {
        var (title, body) = request;

        var newPost = new Post(title, body);

        _dbContext.Posts.Add(newPost);

        return newPost;
    }
}