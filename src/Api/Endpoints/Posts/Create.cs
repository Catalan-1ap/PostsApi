using Api.Common;
using Api.Processors;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using Infrastructure.Common;


namespace Api.Endpoints.Posts;


public sealed class CreateRequest
{
    [FromClaim(Claims.Id)]
    public string UserId { get; init; } = null!;
    public string Title { get; init; } = null!;
    public string Body { get; init; } = null!;
}


public sealed class CreateResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Body { get; init; } = null!;
}


public sealed class CreateValidator : Validator<CreateRequest>
{
    public CreateValidator()
    {
        RuleFor(x => x.Title).ApplyTitleRules();

        RuleFor(x => x.Body).ApplyBodyRules();
    }
}


public sealed class CreateEndpoint : BaseEndpoint<CreateRequest, CreateResponse>
{
    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public override void Configure()
    {
        Post(ApiRoutes.Posts.Create);
        PostProcessors(new SaveChangesPostProcessor<CreateRequest, CreateResponse>());

        Summary(x =>
        {
            x.Response<CreateResponse>(StatusCodes.Status201Created);
        });
    }


    public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
    {
        var newPost = new Post
        {
            Title = req.Title,
            Body = req.Body,
            OwnerId = req.UserId
        };

        ApplicationDbContext.Posts.Add(newPost);

        var res = new CreateResponse
        {
            Id = newPost.Id,
            Title = newPost.Title,
            Body = newPost.Body
        };

        await SendCreatedAtAsync<GetByIdEndpoint>(new { id = res.Id }, res, cancellation: ct);
    }
}