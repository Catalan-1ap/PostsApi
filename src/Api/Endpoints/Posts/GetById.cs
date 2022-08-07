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


public sealed class GetByIdEndpoint : Endpoint<GetByIdRequest, GetByIdResponse>
{
    private readonly IApplicationDbContext _dbContext;


    public GetByIdEndpoint(IApplicationDbContext dbContext) => _dbContext = dbContext;


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


    public override async Task HandleAsync(GetByIdRequest req, CancellationToken ct)
    {
        await PrepareAsync(req, ct);

        var post = await _dbContext.Posts
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
            .SingleAsync(ct);

        await SendOkAsync(post, ct);
    }


    private async Task PrepareAsync(GetByIdRequest req, CancellationToken ct)
    {
        await ValidationRules.PostShouldExistsAsync(req.Id, _dbContext, ct);
    }
}