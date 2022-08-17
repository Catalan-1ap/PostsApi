using Core.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;


namespace Infrastructure.Common;


public static class Extensions
{
    public static string FsAvatarsPath(this IHostEnvironment environment) => Path.Combine(
        environment.ContentRootPath,
        "StaticFiles",
        "Avatars"
    );


    public static SeveralErrorsException ToSeveralErrorsException(this IdentityResult identityResult) =>
        new(identityResult.Errors.Select(x => x.Description));
}