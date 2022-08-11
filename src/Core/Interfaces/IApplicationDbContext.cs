using Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace Core.Interfaces;


public interface IApplicationDbContext
{
    DbSet<Post> Posts { get; }
    DbSet<Like> Likes { get; }
    DbSet<Dislike> Dislikes { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task SaveChangesAsync(CancellationToken ct);
}