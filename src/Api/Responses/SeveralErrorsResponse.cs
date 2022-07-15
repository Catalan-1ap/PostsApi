namespace Api.Responses;


public sealed record SeveralErrorsResponse(IEnumerable<string> Errors);