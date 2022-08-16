using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace Infrastructure.Persistence;


internal class ApplicationDbContextDesignTimeFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder();
        var optionsBuilder = PostgresOptionsFactory.Make(new()
        {
            Host = "localhost",
            Port = 5432,
            User = "sa",
            Password = "pass"
        });

        optionsBuilder.Invoke(builder);

        return new(builder.Options);
    }
}