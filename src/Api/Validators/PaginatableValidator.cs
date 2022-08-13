using Core.Models;
using FastEndpoints;
using FluentValidation;


namespace Api.Validators;


public sealed class PaginatableValidator : Validator<IPaginatable>
{
    public PaginatableValidator()
    {
        RuleFor(x => x.Page)
            .NotEmpty()
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .NotEmpty()
            .LessThanOrEqualTo(50);
    }
}