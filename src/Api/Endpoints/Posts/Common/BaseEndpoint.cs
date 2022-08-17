using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Api.Endpoints.Posts.Common;


public abstract class BaseEndpoint<TRequest, TResponse> : SharedBaseEndpoint<TRequest, TResponse>
    where TRequest : notnull, new()
    where TResponse : notnull, new()
{
    public abstract IIdentityService IdentityService { get; init; }


    public async Task<(Like? like, Dislike? dislike)> SelectOpinion(
        Guid postId,
        string userId,
        CancellationToken ct
    )
    {
        var rate = await ApplicationDbContext.Posts
            .Where(x => x.Id == postId)
            .Select(
                x => new
                {
                    Like = x.Likes.FirstOrDefault(l => l.UserId == userId),
                    Dislike = x.Dislikes.FirstOrDefault(d => d.UserId == userId)
                }
            )
            .FirstOrDefaultAsync(ct);

        return (rate?.Like, rate?.Dislike);
    }


    public async Task<Post?> PostShouldExistsAsync(Guid id, CancellationToken ct)
    {
        var post = await ApplicationDbContext.Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        return post;
    }


    protected bool IsOwner(Post post, string userId) => post.OwnerId == userId;


    protected async Task<bool> IsAdminAsync(DeleteRequest req) =>
        await IdentityService.IsInRoleAsync(new() { Id = req.UserId }, Role.Admin);
}