using System.Text.Json.Serialization;
using Api.Common;
using Api.Processors;
using Api.Responses;
using Core.Interfaces;
using FastEndpoints;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;


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


public sealed class UpdateEndpoint : BaseEndpoint<UpdateRequest, EmptyResponse>
{
    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


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


    public override async Task OnAfterValidateAsync(UpdateRequest req, CancellationToken ct = default)
    {
        var post = await PostShouldExistsAsync(req.Id, ct);

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
        var post = await ApplicationDbContext.Posts
            .FirstAsync(x => x.Id == req.Id, ct);

        post.Title = req.Title;
        post.Body = req.Body;

        ApplicationDbContext.Posts.Update(post);

        await SendOkAsync(ct);
    }
}