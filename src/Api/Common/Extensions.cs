using System.ComponentModel.DataAnnotations;
using Api.Responses;
using ValidationException = FluentValidation.ValidationException;


namespace Api.Common;


public static class Extensions
{
    public static ValidationErrorResponse ToValidationErrorResponse(this ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, f => f.ToArray());

        return new(errors);
    }


    public static void ValidateDataAnnotations<T>(this T target) where T : class
    {
        Validator.ValidateObject(target, new(target));
    }
}