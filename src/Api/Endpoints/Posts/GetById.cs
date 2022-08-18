using Api.Endpoints.Posts.Common;
using Api.Responses;
using Core;
using Core.Interfaces;
using FastEndpoints;
using LinqKit;
using Microsoft.EntityFrameworkCore;


namespace Api.Endpoints.Posts;


public sealed class GetByIdRequest
{
    public Guid PostId { get; init; }
}


public sealed class GetByIdResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string? CoverUrl { get; set; }
    public string Body { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public int Rating { get; init; }
    public OwnerInfo Owner { get; init; } = null!;


    public sealed class OwnerInfo
    {
        public string Id { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string? AvatarUri { get; init; }
    }
}


public sealed class GetPostByIdValidator : Validator<GetByIdRequest>
{
    public GetPostByIdValidator()
    {
        RuleFor(x => x.PostId).ApplyIdRules();
    }
}


public sealed class GetByIdEndpoint : BaseEndpoint<GetByIdRequest, GetByIdResponse>
{
    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public override void Configure()
    {
        Get(ApiRoutes.Posts.GetById);
        AllowAnonymous();

        Summary(
            x =>
            {
                x.Response<GetByIdResponse>();
                x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
            }
        );
    }


    public override async Task OnAfterValidateAsync(GetByIdRequest req, CancellationToken ct = default)
    {
        var post = await PostShouldExistsAsync(req.PostId, ct);

        if (post is null)
            await SendNotFoundAsync(ct);
    }


    public override async Task HandleAsync(GetByIdRequest req, CancellationToken ct)
    {
        var post = await ApplicationDbContext.Posts
            .AsNoTracking()
            .AsExpandable()
            .Where(x => x.Id == req.PostId)
            .Select(
                x => new GetByIdResponse
                {
                    Id = x.Id,
                    Body = x.Body,
                    Title = x.Title,
                    CoverUrl = x.CoverUrl,
                    CreatedAt = x.CreatedAt,
                    UpdatedAt = x.UpdatedAt,
                    Rating = Core.Entities.Post.RatingExpression.Invoke(x),
                    Owner = new()
                    {
                        Id = x.Owner.Id,
                        UserName = x.Owner.UserName,
                        AvatarUri = x.Owner.AvatarUrl
                    }
                }
            )
            .FirstAsync(ct);

        await SendOkAsync(post, ct);
    }
}