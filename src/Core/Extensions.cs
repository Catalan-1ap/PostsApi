using FluentValidation;


namespace Core;


public static class Extensions
{
    public static IRuleBuilderOptions<T, string>
        MaximumLengthWithMessage<T>(this IRuleBuilder<T, string> builder, int maxLength) =>
        builder
            .MaximumLength(maxLength)
            .WithMessage("Max length is: {MaxLength}, entered: {TotalLength}");
}