using Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace Core.Interfaces;


public interface IApplicationDbContext
{
    DbSet<Post> Posts { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken);
}