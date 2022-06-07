using Domain;
using Microsoft.EntityFrameworkCore;


namespace Application.Interfaces;


public interface IApplicationDbContext
{
    DbSet<Post> Posts { get; }

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}