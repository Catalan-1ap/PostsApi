using Api.Responses;
using Core.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;


namespace Api.Common;


public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;


    public ExceptionMiddleware(RequestDelegate next) => _next = next;


    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }


    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        IActionResult result = exception switch
        {
            NotFoundException e => new NotFoundObjectResult(new SingleErrorResponse(e.Message)),
            ValidationException e => new BadRequestObjectResult(e.ConvertValidationExceptionToValidationErrorResponse()),
            SeveralErrorsException e => new BadRequestObjectResult(new SeveralErrorsResponse(e.Errors)),
            BusinessException e => new BadRequestObjectResult(new SingleErrorResponse(e.Message)),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };

        await result.ExecuteResultAsync(new()
        {
            HttpContext = context
        });
    }
}