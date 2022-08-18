using System.ComponentModel.DataAnnotations;
using Api.Responses;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using FluentValidation;
using FluentValidation.Results;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;


namespace Api.Common;


public static class Extensions
{
    public static async Task<Paginated<T>> Paginate<T>(
        this IQueryable<T> queryable,
        IPaginatable paginatable,
        CancellationToken ct = default
    )
    {
        var size = paginatable.PageSize;
        var page = paginatable.Page;
        var total = await queryable.CountAsync(ct);
        var pages = (int)Math.Ceiling(total / (double)size);

        if (page > pages)
            throw new NotFoundException();

        var data = await queryable
            .Skip((page - 1) * size)
            .Take(size)
            .ToArrayAsync(ct);

        return new()
        {
            Page = page,
            PageSize = size,
            TotalPages = pages,
            Data = data
        };
    }


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


    public static void MaximumLengthWithMessage<T>(
        this IRuleBuilder<T, string> builder,
        int maxLength
    ) => builder
        .MaximumLength(maxLength)
        .WithMessage("Max length is: {MaxLength}, entered: {TotalLength}");


    public static void MinimumLengthWithMessage<T>(
        this IRuleBuilder<T, string> builder,
        int minLength
    ) => builder
        .MinimumLength(minLength)
        .WithMessage("Min length is: {MinLength}, entered: {TotalLength}");


    public static IRuleBuilderOptions<T, TProperty> NotEmptyWithMessage<T, TProperty>(this IRuleBuilder<T, TProperty> builder) =>
        builder
            .NotEmpty()
            .WithMessage("Must not be empty");


    public static IRuleBuilderOptions<T, IFormFile> MaxSize<T>(this IRuleBuilder<T, IFormFile> builder, int maxSize) =>
        builder
            .Must(file => file.Length < maxSize)
            .WithMessage($"File is too large, accepted length is {maxSize}");


    public static IRuleBuilderOptions<T, IFormFile> ExtensionOneOf<T>(
        this IRuleBuilder<T, IFormFile> builder,
        params string[] possibleExtensions
    ) =>
        builder
            .Must(file => FileUtilities.ValidExtension(Path.GetExtension(file.FileName), possibleExtensions))
            .WithMessage($"Extension is invalid. Possible extensions: {string.Join(", ", possibleExtensions)}");


    public static IRuleBuilderOptions<T, IFormFile> SignatureMatchToExtension<T>(this IRuleBuilder<T, IFormFile> builder) =>
        builder
            .Must(file =>
            {
                var fileExtension = Path.GetExtension(file.FileName);
                using var reader = file.OpenReadStream();

                return FileUtilities.SignatureMatchToExtension(fileExtension, reader);
            })
            .WithMessage("Signature/extension doesn't correspond each other");


    public static IRuleBuilderOptions<T, IFormFile> MaxResolution<T>(
        this IRuleBuilder<T, IFormFile> builder,
        ImageSize size
    ) => builder
        .Must(file =>
        {
            using var reader = file.OpenReadStream();

            return FileUtilities.MaximumResolution(reader, size.Width, size.Height);
        })
        .WithMessage($"Maximum resolution is {size.Width}x{size.Height}");


    public static IRuleBuilderOptions<T, string> UniqueUsername<T>(
        this IRuleBuilder<T, string> builder,
        IIdentityService identityService
    ) =>
        builder
            .MustAsync(async (username, _) => await identityService.IsUsernameUniqueAsync(username))
            .WithMessage("Must be unique");


    public static IRuleBuilderOptions<T, string> UniqueEmail<T>(
        this IRuleBuilder<T, string> builder,
        IIdentityService identityService
    ) =>
        builder
            .MustAsync(async (email, _) => await identityService.IsEmailUniqueAsync(email))
            .WithMessage("Must be unique");
}