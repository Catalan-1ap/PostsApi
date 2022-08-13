using Core.Entities;
using Core.Interfaces;
using Infrastructure.Persistence.Interceptors;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Persistence;


internal sealed class ApplicationDbContext : IdentityDbContext<User, Role, string>, IApplicationDbContext
{
    private readonly AuditableSaveChangesInterceptor _auditableSaveChangesInterceptor;

    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<Like> Likes { get; set; } = null!;
    public DbSet<Dislike> Dislikes { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;


    public ApplicationDbContext(
        DbContextOptions options,
        AuditableSaveChangesInterceptor auditableSaveChangesInterceptor
    ) : base(options) =>
        _auditableSaveChangesInterceptor = auditableSaveChangesInterceptor;


    public new async Task SaveChangesAsync(CancellationToken ct)
    {
        await base.SaveChangesAsync(ct);
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(_auditableSaveChangesInterceptor);


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}