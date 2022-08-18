﻿using Api.Endpoints.Posts.Common;
using Api.Responses;
using Core;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using Infrastructure.Common;
using Newtonsoft.Json;


namespace Api.Endpoints.Posts;


public sealed class DislikeRequest
{
    [FromClaim(Claims.Id)]
    public string UserId { get; init; } = null!;
    [JsonIgnore]
    public Guid PostId { get; init; }
}


public sealed class DislikeValidator : Validator<DislikeRequest>
{
    public DislikeValidator()
    {
        RuleFor(x => x.PostId).ApplyIdRules();
    }
}


public sealed class DislikeEndpoint : BaseEndpoint<DislikeRequest, EmptyResponse>
{
    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public override void Configure()
    {
        Post(ApiRoutes.Posts.Dislike);

        Summary(
            x =>
            {
                x.Summary = "Dislike selected post, if dislike exists it will be cancelled, if like exists it will be replaced";
                x.Response();
                x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
            }
        );
    }


    public override async Task OnAfterValidateAsync(DislikeRequest req, CancellationToken ct = default)
    {
        var post = await PostShouldExistsAsync(req.PostId, ct);

        if (post is null)
            await SendNotFoundAsync(ct);
    }


    public override async Task HandleAsync(DislikeRequest req, CancellationToken ct)
    {
        var existed = await SelectOpinion(req.PostId, req.UserId, ct);

        if (existed.dislike is not null)
        {
            ApplicationDbContext.Dislikes.Remove(existed.dislike);
            await SendOkAsync(ct);
            return;
        }

        if (existed.like is not null)
            ApplicationDbContext.Likes.Remove(existed.like);

        var dislike = new Dislike
        {
            PostId = req.PostId,
            UserId = req.UserId
        };

        ApplicationDbContext.Dislikes.Add(dislike);

        await ApplicationDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}