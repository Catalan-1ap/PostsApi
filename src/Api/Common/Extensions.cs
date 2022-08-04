using System.ComponentModel.DataAnnotations;
using Api.Responses;
using FluentValidation;
using FluentValidation.Results;


namespace Api.Common;


public static class Extensions
{
    public static ValidationErrorResponse ToValidationErrorResponse(this IEnumerable<ValidationFailure> failures)
    {
        var errors = failures
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, f => f.ToArray());

        return new(errors);
    }


    public static void ValidateDataAnnotations<T>(this T target) where T : class
    {
        Validator.ValidateObject(target, new(target));
    }


    public static IRuleBuilderOptions<T, string> MaximumLengthWithMessage<T>(
        this IRuleBuilder<T, string> builder,
        int maxLength
    ) => builder
        .MaximumLength(maxLength)
        .WithMessage("Max length is: {MaxLength}, entered: {TotalLength}");


    public static IRuleBuilderOptions<T, string> MinimumLengthWithMessage<T>(
        this IRuleBuilder<T, string> builder,
        int minLength
    ) => builder
        .MinimumLength(minLength)
        .WithMessage("Min length is: {MinLength}, entered: {TotalLength}");


    public static IRuleBuilderOptions<T, string> NotEmptyWithMessage<T>(this IRuleBuilder<T, string> builder) =>
        builder
            .NotEmpty()
            .WithMessage("Must not be empty");
}