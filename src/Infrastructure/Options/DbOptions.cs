namespace Infrastructure.Options;


public sealed record DbOptions(string Host, int Port, string Username, string Password);