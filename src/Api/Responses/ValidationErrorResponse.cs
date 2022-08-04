namespace Api.Responses;


public sealed record ValidationErrorResponse(IReadOnlyDictionary<string, string[]> Errors);