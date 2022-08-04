using System.Text.Json.Serialization;
using Api.Common;
using Api.Processors;
using Api.Responses;
using Core.Interfaces;
using FastEndpoints;


namespace Api.Endpoints.Posts;


public sealed class UpdateRequest
{
    [JsonIgnore]
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Body { get; init; } = null!;
}


public sealed class UpdateValidator : Validator<UpdateRequest>
{
    public UpdateValidator()
    {
        RuleFor(x => x.Id).ApplyIdRules();

        RuleFor(x => x.Title).ApplyTitleRules();

        RuleFor(x => x.Body).ApplyBodyRules();
    }
}


public sealed class UpdateEndpoint : Endpoint<UpdateRequest, EmptyResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;


    public UpdateEndpoint(ICurrentUserService currentUserService, IApplicationDbContext dbContext)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
    }


    public override void Configure()
    {
        Put(ApiRoutes.Posts.Update);
        PostProcessors(new SaveChangesPostProcessor<UpdateRequest, EmptyResponse>());

        Summary(x =>
        {
            x.Response();
            x.Response<ValidationErrorResponse>(StatusCodes.Status400BadRequest, "Validation error");
            x.Response(StatusCodes.Status401Unauthorized);
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
        });
    }


    public override async Task HandleAsync(UpdateRequest req, CancellationToken ct)
    {
        await Prepare(req, ct);

        var post = await _dbContext.Posts.FindAsync(new object?[] { req.Id }, ct);

        post!.Title = req.Title;
        post.Body = req.Body;

        _dbContext.Posts.Update(post);

        await SendOkAsync(ct);
    }


    private async Task Prepare(UpdateRequest req, CancellationToken ct)
    {
        var post = await ValidationRules.PostShouldExists(req.Id, _dbContext, ct);

        ValidationRules.CurrentUserShouldOwnPost(post, _currentUserService);
    }
}