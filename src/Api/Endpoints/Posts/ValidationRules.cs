using Api.Common;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Core.StorageContracts;
using FluentValidation;


namespace Api.Endpoints.Posts;


internal static class ValidationRules
{
    public static void ApplyIdRules<T>(this IRuleBuilder<T, Guid> builder) =>
        builder
            .NotEmpty();


    public static void ApplyTitleRules<T>(this IRuleBuilder<T, string> builder) =>
        builder
            .NotEmptyWithMessage()
            .MaximumLengthWithMessage(PostStorageContract.TitleMaxLength);


    public static void ApplyBodyRules<T>(this IRuleBuilder<T, string> builder) =>
        builder
            .NotEmptyWithMessage()
            .MaximumLengthWithMessage(PostStorageContract.BodyMaxLength);


    public static async Task<Post> PostShouldExistsAsync(
        Guid id,
        IApplicationDbContext dbContext,
        CancellationToken cancellationToken
    )
    {
        var post = await dbContext.Posts
            .FindAsync(new object[] { id }, cancellationToken);

        return post ?? throw NotFoundException.Make(nameof(Post), id);
    }


    public static void UserShouldOwnPost(Post post, string userId)
    {
        if (post.OwnerId != userId)
            throw new BusinessException("You does not own this post");
    }
}