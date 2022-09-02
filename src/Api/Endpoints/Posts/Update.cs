using System.Text.Json.Serialization;
using Api.Endpoints.Posts.Common;
using Api.Responses;
using Core;
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
    private Post? _post;

    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;
    public IDateTimeService DateTimeService { get; init; } = null!;


    public override void Configure()
    {
        Put(ApiRoutes.Posts.Update);

        Summary(x =>
        {
            x.Response();
            x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
            x.Response(StatusCodes.Status403Forbidden);
        });
    }


    public override async Task OnAfterValidateAsync(UpdateRequest req, CancellationToken ct = default)
    {
        _post = await PostShouldExistsAsync(req.PostId, ct);

        if (_post is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (IsOwner(_post, req.UserId) == false)
            await SendForbiddenAsync(ct);
    }


    public override async Task HandleAsync(UpdateRequest req, CancellationToken ct)
    {
        ApplicationDbContext.Posts.Attach(_post!);

        _post!.Title = req.Title;
        _post.Body = req.Body;

        await ApplicationDbContext.SaveChangesAsync(ct);
        await SendOkAsync(CancellationToken.None);
    }
}