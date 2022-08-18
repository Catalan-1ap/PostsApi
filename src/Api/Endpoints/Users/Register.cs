using Api.Common;
using Api.Options;
using Core;
using Core.Interfaces;
using FastEndpoints;
using FluentValidation;
using Microsoft.Extensions.Options;


namespace Api.Endpoints.Users;


public sealed class RegisterRequest
{
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public IFormFile? Avatar { get; init; }
}


public sealed class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator(IIdentityService identityService, IOptions<AvatarOptions> avatarOptions)
    {
        RuleFor(x => x.UserName)
            .Cascade(CascadeMode.Stop)
            .NotEmptyWithMessage()
            .UniqueUsername(identityService);

        RuleFor(x => x.Email)
            .NotEmptyWithMessage()
            .EmailAddress()
            .UniqueEmail(identityService);

        RuleFor(x => x.Password)
            .NotEmptyWithMessage()
            .MinimumLengthWithMessage(8);

        When(x => x.Avatar is not null, () =>
        {
            var options = avatarOptions.Value;

            RuleFor(x => x.Avatar!)
                .Cascade(CascadeMode.Stop)
                .MaxSize(options.MaxSize)
                .ExtensionOneOf(options.PossibleExtensions)
                .SignatureMatchToExtension()
                .MaxResolution(options.MaxResolution);
        });
    }
}


public sealed class RegisterEndpoint : SharedBaseEndpoint<RegisterRequest, EmptyResponse>
{
    private readonly IIdentityService _identityService;
    private readonly IStaticFilesService _staticFilesService;


    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public RegisterEndpoint(
        IIdentityService identityService,
        IStaticFilesService staticFilesService
    )
    {
        _identityService = identityService;
        _staticFilesService = staticFilesService;
    }


    public override void Configure()
    {
        Post(ApiRoutes.Users.Register);
        AllowFileUploads();
        AllowAnonymous();
        ScopedValidator();

        Summary(
            x =>
            {
                x.Response<LoginResponse>(StatusCodes.Status200OK, "JWT tokens");
            }
        );
    }


    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = await _identityService.RegisterAsync(req.UserName, req.Email, req.Password);

        if (req.Avatar is not null)
        {
            var fileName = await _staticFilesService.SaveAvatar(req.Avatar, user.Id);
            var avatarUri = _staticFilesService.CreateAvatarUri(fileName);

            user.AvatarUrl = avatarUri;
        }

        await ApplicationDbContext.SaveChangesAsync();
        await SendOkAsync();
    }
}