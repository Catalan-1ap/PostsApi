using Api.Common;
using Api.Endpoints.Users.Common;
using Api.Options;
using Core.Interfaces;
using Core.Models;
using FastEndpoints;
using FluentValidation;
using Infrastructure.Common;
using Microsoft.Extensions.Options;


namespace Api.Endpoints.Users;


public sealed class RegisterRequest
{
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public IFormFile Avatar { get; init; } = null!;
}


public sealed class RegisterResponse
{
    public JwtTokens Tokens { get; init; } = null!;
}


public sealed class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmptyWithMessage();

        RuleFor(x => x.Email)
            .NotEmptyWithMessage()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmptyWithMessage()
            .MinimumLengthWithMessage(8);

        RuleFor(x => x.Avatar)
            .NotEmptyWithMessage();
    }
}


public sealed class RegisterEndpoint : BaseEndpoint<RegisterRequest, RegisterResponse>
{
    private readonly AvatarOptions _avatarOptions;
    private readonly IAvatarRepository _avatarRepository;
    private readonly IIdentityService _identityService;
    private readonly IJwtService _jwtService;


    public override IApplicationDbContext ApplicationDbContext { get; init; } = null!;


    public RegisterEndpoint(
        IIdentityService identityService,
        IJwtService jwtService,
        IOptions<AvatarOptions> avatarOptions,
        IAvatarRepository avatarRepository
    )
    {
        _identityService = identityService;
        _jwtService = jwtService;
        _avatarRepository = avatarRepository;
        _avatarOptions = avatarOptions.Value;
    }


    public override void Configure()
    {
        Post(ApiRoutes.Users.Register);
        AllowFileUploads();
        AllowAnonymous();

        Summary(
            x =>
            {
                x.Response<LoginResponse>(StatusCodes.Status200OK, "JWT tokens");
            }
        );
    }


    public override async Task OnAfterValidateAsync(RegisterRequest req, CancellationToken ct = default)
    {
        if (await _identityService.IsUsernameUniqueAsync(req.UserName) == false)
            ThrowError(x => x.UserName, "Must be unique");

        if (await _identityService.IsEmailUniqueAsync(req.Email) == false)
            ThrowError(x => x.Email, "Must be unique");

        if (req.Avatar.Length > _avatarOptions.MaxSize)
            ThrowError(x => x.Avatar, $"File is too large, accepted length is {_avatarOptions.MaxSize}");

        var avatarExtension = Path.GetExtension(req.Avatar.FileName);

        if (FileUtilities.ValidExtension(avatarExtension, _avatarOptions.PossibleExtensions) == false)
            ThrowError(x => x.Avatar, "Extension is invalid");

        await using var avatarStream = req.Avatar.OpenReadStream();

        if (FileUtilities.SignatureMatchToExtension(avatarExtension, avatarStream) == false)
            ThrowError(x => x.Avatar, "Signature/extension doesn't correspond each other");

        if (FileUtilities.MaximumResolution(avatarStream, _avatarOptions.MaxWidth, _avatarOptions.MaxHeight) == false)
            ThrowError(x => x.Avatar, $"Maximum resolution is {_avatarOptions.MaxWidth}x{_avatarOptions.MaxHeight}");
    }


    public override async Task HandleAsync(RegisterRequest req, CancellationToken ct)
    {
        var user = await _identityService.RegisterAsync(req.UserName, req.Email, req.Password);
        var fileName = await _avatarRepository.Save(req.Avatar, user.Id);
        var tokens = await _jwtService.AccessAsync(user);
        var avatarUri = CreateAvatarUri(fileName);

        user.AvatarUrl = avatarUri;

        await ApplicationDbContext.SaveChangesAsync(ct);
        await SendOkAsync(
            new()
            {
                Tokens = tokens
            },
            ct
        );
    }
}