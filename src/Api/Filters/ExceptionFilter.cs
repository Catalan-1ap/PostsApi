using Api.Common;
using Application.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Api.Filters;


public sealed class ExceptionFilter : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        IActionResult? result = context.Exception switch
        {
            BusinessException e => new NotFoundObjectResult(e.Message),
            ValidationException e => new BadRequestObjectResult(ConvertValidationExceptionToResponse(e)),
            _ => null
        };

        if (result is not null)
            context.Result = result;
    }


    private ValidationError ConvertValidationExceptionToResponse(ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
            .ToDictionary(g => g.Key, f => f.ToArray());

        return new(errors);
    }
}