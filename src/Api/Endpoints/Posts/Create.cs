using Api.Common;
using Api.Processors;
using Api.Responses;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;


namespace Api.Endpoints.Posts;


public sealed class CreateRequest
{
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


public sealed class CreateEndpoint : Endpoint<CreateRequest, CreateResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;


    public CreateEndpoint(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }


    public override void Configure()
    {
        Post(ApiRoutes.Posts.Create);
        PostProcessors(new SaveChangesPostProcessor<CreateRequest, CreateResponse>());

        Summary(x =>
        {
            x.Response<CreateResponse>(StatusCodes.Status201Created);
            x.Response<ValidationErrorResponse>(StatusCodes.Status400BadRequest, "Validation error");
            x.Response(StatusCodes.Status401Unauthorized);
        });
    }


    public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
    {
        var currentUser = _currentUserService.UserId;

        var newPost = new Post
        {
            Title = req.Title,
            Body = req.Body,
            OwnerId = currentUser
        };

        _dbContext.Posts.Add(newPost);

        var res = new CreateResponse
        {
            Id = newPost.Id,
            Title = newPost.Title,
            Body = newPost.Body
        };

        await SendCreatedAtAsync<GetByIdEndpoint>(new { id = res.Id }, res, cancellation: ct);
    }
}