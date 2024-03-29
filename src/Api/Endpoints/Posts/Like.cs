﻿using Api.Endpoints.Posts.Common;
using Api.Responses;
using Core;
using Core.Common;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using Newtonsoft.Json;


namespace Api.Endpoints.Posts;


public sealed class LikeRequest
{
    [FromClaim(Claims.Id)]
    public string UserId { get; init; } = null!;
    [JsonIgnore]
    public Guid PostId { get; init; }
}


public sealed class LikeValidator : Validator<LikeRequest>
{
    public LikeValidator()
    {
        RuleFor(x => x.PostId).ApplyIdRules();
    }
}


public sealed class LikeEndpoint : BaseEndpoint<LikeRequest, EmptyResponse>
{
    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public override void Configure()
    {
        Post(ApiRoutes.Posts.Like);

        Summary(x =>
        {
            x.Summary = "Like selected post, if like exists it will be cancelled, if dislike exists it will be replaced";
            x.Response();
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
        });
    }


    public override async Task OnAfterValidateAsync(LikeRequest req, CancellationToken ct = default)
    {
        var post = await PostShouldExistsAsync(req.PostId, ct);

        if (post is null)
            await SendNotFoundAsync(ct);
    }


    public override async Task HandleAsync(LikeRequest req, CancellationToken ct)
    {
        var existed = await SelectOpinionOfPostAsync(req.PostId, req.UserId, ct);

        if (existed.like is not null)
        {
            ApplicationDbContext.PostsLikes.Remove(existed.like);
            await ApplicationDbContext.SaveChangesAsync(ct);
            await SendOkAsync(CancellationToken.None);
            return;
        }

        if (existed.dislike is not null)
            ApplicationDbContext.PostsDislikes.Remove(existed.dislike);

        var like = new PostLike
        {
            PostId = req.PostId,
            UserId = req.UserId
        };

        ApplicationDbContext.PostsLikes.Add(like);

        await ApplicationDbContext.SaveChangesAsync(ct);
        await SendOkAsync(CancellationToken.None);
    }
}