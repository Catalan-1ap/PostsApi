using Core.Exceptions;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Common;


public static class Extensions
{
    public static SeveralErrorsException ToSeveralErrorsException(this IdentityResult identityResult) =>
        new(identityResult.Errors.Select(x => x.Description));
}