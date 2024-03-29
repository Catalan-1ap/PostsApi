﻿using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Npgsql;


namespace Infrastructure.Persistence;


internal static class PostgresOptionsFactory
{
    public static Action<DbContextOptionsBuilder> Make(PostgresOptions postgresOptions) => optionsBuilder =>
    {
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host = postgresOptions.Host,
            Port = postgresOptions.Port!.Value,
            Username = postgresOptions.User,
            Password = postgresOptions.Password,
            Pooling = true
        }.ToString();

        optionsBuilder.UseNpgsql(connectionString);
    };
}