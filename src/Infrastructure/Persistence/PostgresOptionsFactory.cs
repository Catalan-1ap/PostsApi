using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace Infrastructure.Persistence;


internal static class PostgresOptionsFactory
{
    public static Action<DbContextOptionsBuilder> Make(DbOptions dbOptions) => optionsBuilder =>
    {
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = dbOptions.Host,
            Port = dbOptions.Port,
            Username = dbOptions.Username,
            Password = dbOptions.Password
        }.ToString();

        optionsBuilder.UseNpgsql(connectionString);
    };
}