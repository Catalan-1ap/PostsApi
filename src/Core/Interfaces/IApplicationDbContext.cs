using Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace Core.Interfaces;


public interface IApplicationDbContext
{
    DbSet<Post> Posts { get; }
    DbSet<PostLike> PostsLikes { get; }
    DbSet<PostDislike> PostsDislikes { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task SaveChangesAsync(CancellationToken ct = default);
}