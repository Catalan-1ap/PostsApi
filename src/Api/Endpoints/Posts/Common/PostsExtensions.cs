using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Api.Endpoints.Posts.Common;


public static class PostsExtensions
{
    public static async Task<(Like? like, Dislike? dislike)> SelectOpinion(
        this IApplicationDbContext dbContext,
        Guid postId,
        string userId,
        CancellationToken ct
    )
    {
        var rate = await dbContext.Posts
            .Where(x => x.Id == postId)
            .Select(x => new
            {
                Like = x.Likes.FirstOrDefault(l => l.UserId == userId),
                Dislike = x.Dislikes.FirstOrDefault(d => d.UserId == userId)
            })
            .FirstOrDefaultAsync(ct);

        return (rate?.Like, rate?.Dislike);
    }
}