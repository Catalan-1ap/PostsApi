using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.StorageContracts;
using FluentValidation;
using Microsoft.EntityFrameworkCore;


namespace Core.Features.Posts;


internal static class ValidationRules
{
    public static void ApplyTitleRules<T>(this IRuleBuilder<T, string> builder) =>
        builder
            .NotEmptyWithMessage()
            .MaximumLengthWithMessage(PostStorageContract.TitleMaxLength);


    public static void ApplyBodyRules<T>(this IRuleBuilder<T, string> builder) =>
        builder
            .NotEmptyWithMessage()
            .MaximumLengthWithMessage(PostStorageContract.BodyMaxLength);


    public static async Task<bool> PostShouldExists(
        Guid id,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var post = await dbContext.Posts
            .AsNoTracking()
            .SingleOrDefaultAsync(post => post.Id == id, cancellationToken);

        return post is null
            ? throw NotFoundException.Make(nameof(Post), id)
            : true;
    }
}