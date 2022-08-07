using System.Text.Json.Serialization;
using Api.Common;
using Api.Processors;
using Api.Responses;
using Core.Interfaces;
using FastEndpoints;
using Infrastructure.Common;


namespace Api.Endpoints.Posts;


public sealed class UpdateRequest
{
    [FromClaim(Claims.Id)]
    public string UserId { get; init; } = null!;
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
    private readonly IApplicationDbContext _dbContext;


    public UpdateEndpoint(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public override void Configure()
    {
        Put(ApiRoutes.Posts.Update);
        PostProcessors(new SaveChangesPostProcessor<UpdateRequest, EmptyResponse>());

        Summary(x =>
        {
            x.Response();
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
        });
    }


    public override async Task HandleAsync(UpdateRequest req, CancellationToken ct)
    {
        await PrepareAsync(req, ct);

        var post = await _dbContext.Posts.FindAsync(new object?[] { req.Id }, ct);

        post!.Title = req.Title;
        post.Body = req.Body;

        _dbContext.Posts.Update(post);

        await SendOkAsync(ct);
    }


    private async Task PrepareAsync(UpdateRequest req, CancellationToken ct)
    {
        var post = await ValidationRules.PostShouldExistsAsync(req.Id, _dbContext, ct);

        ValidationRules.UserShouldOwnPost(post, req.UserId);
    }
}