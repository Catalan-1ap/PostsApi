using Application.Interfaces;
using Domain;
using Domain.NonDomainEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence;


internal sealed class ApplicationDbContext : IdentityDbContext<User>, IApplicationDbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    public ApplicationDbContext(DbContextOptions options) : base(options) { }


    public new async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}