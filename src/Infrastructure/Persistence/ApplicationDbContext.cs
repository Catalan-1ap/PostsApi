using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence;


internal sealed class ApplicationDbContext : IdentityDbContext<User, Role, string>, IApplicationDbContext
{
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    public ApplicationDbContext(DbContextOptions options) : base(options) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}