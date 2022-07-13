namespace Infrastructure.Options;


public sealed record PostgresOptions(string Host, int Port, string Username, string Password);