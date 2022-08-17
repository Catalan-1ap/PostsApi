using System.ComponentModel.DataAnnotations;
using Api.Responses;
using Core.Exceptions;
using Core.Models;
using FluentValidation;
using FluentValidation.Results;
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
}