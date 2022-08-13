using System.Text.Json.Serialization;
using Api.Common;
using Api.Endpoints.Posts.Common;
using Api.Processors;
using Api.Responses;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using Infrastructure.Common;


namespace Api.Endpoints.Posts;


public sealed class UpdateRequest
{
    [FromClaim(Claims.Id)]
    public string UserId { get; init; } = null!;
    [JsonIgnore]
    public Guid PostId { get; init; }
    public string Title { get; init; } = null!;
    public string Body { get; init; } = null!;
}


public sealed class UpdateValidator : Validator<UpdateRequest>
{
    public UpdateValidator()
    {
        RuleFor(x => x.PostId).ApplyIdRules();

        RuleFor(x => x.Title).ApplyTitleRules();

        RuleFor(x => x.Body).ApplyBodyRules();
    }
}


public sealed class UpdateEndpoint : BaseEndpoint<UpdateRequest, EmptyResponse>
{
    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;
    public IDateTimeService DateTimeService { get; init; } = null!;


    public override void Configure()
    {
        Put(ApiRoutes.Posts.Update);
        PostProcessors(new SaveChangesPostProcessor<UpdateRequest, EmptyResponse>());

        Summary(x =>
        {
            x.Response();
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
            x.Response(StatusCodes.Status403Forbidden);
        });
    }


    public override async Task OnAfterValidateAsync(UpdateRequest req, CancellationToken ct = default)
    {
        var post = await PostShouldExistsAsync(req.PostId, ct);

        if (post is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (IsOwner(post, req.UserId) == false)
            await SendForbiddenAsync(ct);
    }


    public override async Task HandleAsync(UpdateRequest req, CancellationToken ct)
    {
        var post = new Post
        {
            Id = req.PostId
        };

        ApplicationDbContext.Posts.Attach(post);

        post.Title = req.Title;
        post.Body = req.Body;

        await SendOkAsync(ct);
    }
}