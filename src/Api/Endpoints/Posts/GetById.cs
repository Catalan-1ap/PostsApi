using Api.Common;
using Api.Responses;
using Core.Interfaces;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;


namespace Api.Endpoints.Posts;


public sealed class GetByIdRequest
{
    public Guid Id { get; init; }
}


public sealed class GetByIdResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Body { get; init; } = null!;
    public OwnerInfo Owner { get; init; } = null!;


    public sealed class OwnerInfo
    {
        public string Id { get; init; } = null!;
        public string UserName { get; init; } = null!;
    }
}


public sealed class GetPostByIdValidator : Validator<GetByIdRequest>
{
    public GetPostByIdValidator()
    {
        RuleFor(x => x.Id).ApplyIdRules();
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

        Summary(x =>
        {
            x.Response<GetByIdResponse>();
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
        });
    }


    public override async Task OnAfterValidateAsync(GetByIdRequest req, CancellationToken ct = new())
    {
        var post = await PostShouldExistsAsync(req.Id, ct);

        if (post is null)
            await SendNotFoundAsync(ct);
    }


    public override async Task HandleAsync(GetByIdRequest req, CancellationToken ct)
    {
        var post = await ApplicationDbContext.Posts
            .AsNoTracking()
            .Where(x => x.Id == req.Id)
            .Select(x => new GetByIdResponse
            {
                Id = x.Id,
                Body = x.Body,
                Title = x.Title,
                Owner = new()
                {
                    Id = x.Owner.Id,
                    UserName = x.Owner.UserName
                }
            })
            .FirstAsync(ct);

        await SendOkAsync(post, ct);
    }
}