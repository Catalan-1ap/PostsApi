using Api.Common;
using Api.Endpoints.Posts.Common;
using Api.Options;
using Core;
using Core.Common;
using Core.Entities;
using Core.Interfaces;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Options;


namespace Api.Endpoints.Posts;


public sealed class CreateRequest
{
    [FromClaim(Claims.Id)]
    public string UserId { get; init; } = null!;
    public string Title { get; init; } = null!;
    public IFormFile? CoverImage { get; set; }
    public string? LeadBody { get; init; }
    public string Body { get; init; } = null!;
}


public sealed class CreateResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = null!;
    public string Body { get; init; } = null!;
    public string? CoverUrl { get; set; }
    public string? LeadBody { get; init; }
}


public sealed class CreateValidator : Validator<CreateRequest>
{
    public CreateValidator(IOptions<CoverImageOptions> coverOptions)
    {
        RuleFor(x => x.Title).ApplyTitleRules();

        When(x => x.CoverImage is not null, () =>
        {
            var options = coverOptions.Value;

            RuleFor(x => x.CoverImage!)
                .Cascade(CascadeMode.Stop)
                .MaxSize(options.MaxSize)
                .ExtensionOneOf(options.PossibleExtensions)
                .SignatureMatchToExtension()
                .MaxResolution(options.MaxResolution);
        });

        RuleFor(x => x.LeadBody).ApplyLeadBodyRules();

        RuleFor(x => x.Body).ApplyBodyRules();
    }
}


public sealed class CreateEndpoint : BaseEndpoint<CreateRequest, CreateResponse>
{
    private readonly IStaticFilesService _staticFilesService;

    public override IIdentityService IdentityService { get; init; } = null!;
    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public CreateEndpoint(IStaticFilesService staticFilesService) =>
        _staticFilesService = staticFilesService;


    public override void Configure()
    {
        Post(ApiRoutes.Posts.Create);
        AllowFileUploads();
        ScopedValidator();

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
            LeadBody = req.LeadBody,
            OwnerId = req.UserId
        };

        ApplicationDbContext.Posts.Add(newPost);

        if (req.CoverImage is not null)
        {
            var fileName = await _staticFilesService.SaveCoverAsync(req.CoverImage, newPost.Id.ToString());

            newPost.CoverImageName = fileName;
        }

        var res = new CreateResponse
        {
            Id = newPost.Id,
            Title = newPost.Title,
            Body = newPost.Body,
            LeadBody = newPost.LeadBody,
            CoverUrl = newPost.CoverImageName
        };

        await ApplicationDbContext.SaveChangesAsync(ct);
        await SendCreatedAtAsync<GetByIdEndpoint>(new { id = res.Id }, res, cancellation: CancellationToken.None);
    }
}