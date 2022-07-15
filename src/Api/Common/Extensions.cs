using Api.Responses;
using FluentValidation;


namespace Api.Common;


public static class Extensions
{
    public static Dictionary<string, string> GetEnvironmentVariables(this string[] searchedEnvs)
    {
        var envs = searchedEnvs
            .ToDictionary(x => x, Environment.GetEnvironmentVariable);

        var notDefinedEnvs = envs
            .Where(pair => pair.Value is null)
            .ToArray();

        if (notDefinedEnvs.Any())
        {
            var errors = notDefinedEnvs
                .Select(x => $"Required environment variable \"{x.Key}\" isn't defined");
            var errorDescription = string.Join(Environment.NewLine, errors);

            throw new(errorDescription);
        }

        return envs!;
    }


    public static ValidationErrorResponse ConvertValidationExceptionToValidationErrorResponse(this ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, f => f.ToArray());

        return new(errors);
    }
}