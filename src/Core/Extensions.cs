using FluentValidation;


namespace Core;


public static class Extensions
{
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