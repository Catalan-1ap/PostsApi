using Api.Common;
using Api.Endpoints.Posts.Common;
using Api.Responses;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using Infrastructure.Common;
using Newtonsoft.Json;


namespace Api.Endpoints.Posts;


public sealed class DeleteRequest
{
    [FromClaim(Claims.Id)]
    public string UserId { get; init; } = null!;
    [JsonIgnore]
    public Guid PostId { get; init; }
}


public sealed class DeleteValidator : Validator<DeleteRequest>
{
    public DeleteValidator()
    {
        RuleFor(x => x.PostId).ApplyIdRules();
    }
}


public sealed class DeleteEndpoint : BaseEndpoint<DeleteRequest, EmptyResponse>
{
    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public override void Configure()
    {
        Delete(ApiRoutes.Posts.Delete);

        Summary(
            x =>
            {
                x.Response();
                x.Response<SingleErrorResponse>(StatusCodes.Status404NotFound);
                x.Response(StatusCodes.Status403Forbidden);
            }
        );
    }


    public override async Task OnAfterValidateAsync(DeleteRequest req, CancellationToken ct = default)
    {
        var post = await PostShouldExistsAsync(req.PostId, ct);

        if (post is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (IsOwner(post, req.UserId) == false && await IsAdminAsync(req) == false)
            await SendForbiddenAsync(ct);
    }


    public override async Task HandleAsync(DeleteRequest req, CancellationToken ct)
    {
        var post = new Post
        {
            Id = req.PostId
        };

        ApplicationDbContext.Posts.Remove(post);

        await ApplicationDbContext.SaveChangesAsync(ct);
        await SendOkAsync(ct);
    }
}