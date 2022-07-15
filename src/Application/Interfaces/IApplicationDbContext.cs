using Domain;
using Domain.NonDomainEntities;
using Microsoft.EntityFrameworkCore;


namespace Application.Interfaces;


public interface IApplicationDbContext
{
    DbSet<Post> Posts { get; }
    DbSet<RefreshToken> RefreshTokens { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}