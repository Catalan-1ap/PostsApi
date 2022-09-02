using Api.Common;
using Core.StorageContracts;
using FluentValidation;


namespace Api.Endpoints.Posts;


internal static class ValidationRules
{
    public static void ApplyIdRules<T>(this IRuleBuilder<T, Guid> builder) =>
        builder
            .NotEmpty();


    public static void ApplyTitleRules<T>(this IRuleBuilder<T, string> builder) =>
        builder
            .NotEmptyWithMessage()
            .MaximumLengthWithMessage(PostStorageContract.TitleMaxLength);


    public static void ApplyLeadBodyRules<T>(this IRuleBuilder<T, string?> builder) =>
        builder
            .MaximumLengthWithMessage(PostStorageContract.LeadBodyMaxLength);


    public static void ApplyBodyRules<T>(this IRuleBuilder<T, string> builder) =>
        builder
            .NotEmptyWithMessage()
            .MaximumLengthWithMessage(PostStorageContract.BodyMaxLength);
}