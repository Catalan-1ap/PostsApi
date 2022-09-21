using Api.Common;
using Api.Endpoints.Posts.Common;
using Api.Responses;
using Api.Validators;
using Core;
using Core.Interfaces;
using Core.Models;
using FastEndpoints;
using LinqKit;
using Microsoft.EntityFrameworkCore;


namespace Api.Endpoints.Posts;


public sealed class GetAllRequest : IPaginatable
{
    [QueryParam]
    [BindFrom("p")]
    public int Page { get; init; } = 1;

    [QueryParam]
    [BindFrom("s")]
    public int PageSize { get; init; } = 15;
}


public static class GetAllResponse
{
    public sealed class ShortPost
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = null!;
        public string? LeadBody { get; init; }
        public string? CoverImageName { get; set; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public int Rating { get; init; }
        public GetByIdResponse.OwnerInfo Owner { get; init; } = null!;
    }
}


public sealed class GetAllValidator : Validator<GetAllRequest>
{
    public GetAllValidator() =>
        Include(new PaginatableValidator());
}


public sealed class GetAll : BaseEndpoint<GetAllRequest, Paginated<GetAllResponse.ShortPost>>
{
    private readonly IStaticFilesService _staticFilesService;

    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public GetAll(IStaticFilesService staticFilesService) =>
        _staticFilesService = staticFilesService;


    public override void Configure()
    {
        Get(ApiRoutes.Posts.GetAll);
        AllowAnonymous();

        Summary(x =>
        {
            x.Response<GetByIdResponse>();
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
            x.Params[nameof(GetAllRequest.PageSize)] = "Page size";
            x.Params[nameof(GetAllRequest.Page)] = "Page number";
        });
    }


    public override async Task HandleAsync(GetAllRequest req, CancellationToken ct)
    {
        var paginated = await ApplicationDbContext.Posts
            .AsNoTracking()
            .AsExpandable()
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.UpdatedAt)
            .Select(x => new GetAllResponse.ShortPost
            {
                Id = x.Id,
                Title = x.Title,
                CoverImageName = x.CoverImageName,
                LeadBody = x.LeadBody,
                CreatedAt = x.CreatedAt,
                UpdatedAt = x.UpdatedAt,
                Rating = Core.Entities.Post.RatingExpression.Invoke(x),
                Owner = new()
                {
                    Id = x.Owner.Id,
                    UserName = x.Owner.UserName,
                    AvatarImageName = x.Owner.AvatarImageName
                }
            })
            .PaginateAsync(req, ct);

        foreach (var shortPost in paginated.Data)
        {
            shortPost.CoverImageName = shortPost.CoverImageName.ReplaceIfNotNull(_staticFilesService.CreateCoverUri);
            shortPost.Owner.AvatarImageName =
                shortPost.Owner.AvatarImageName.ReplaceIfNotNull(_staticFilesService.CreateAvatarUri);
        }

        await SendOkAsync(paginated, ct);
    }
}