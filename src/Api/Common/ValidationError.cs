namespace Api.Common;


public sealed record ValidationError(IReadOnlyDictionary<string, string[]> Errors);