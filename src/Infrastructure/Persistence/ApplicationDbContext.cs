using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence;


internal sealed class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Post> Posts { get; set; } = null!;


    public ApplicationDbContext(DbContextOptions options) : base(options) { }


    public new async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await base.SaveChangesAsync(cancellationToken);
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}