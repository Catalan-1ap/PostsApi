using Api.Responses;
using Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
            if (httpContext.Response.HasStarted)
                return;

            await HandleExceptionAsync(httpContext, ex);
        }
    }


    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        IActionResult result = exception switch
        {
            NotFoundException e => new NotFoundResult(),
            SeveralErrorsException e => new BadRequestObjectResult(new SeveralErrorsResponse(e.Errors)),
            BusinessException e => new BadRequestObjectResult(new SingleErrorResponse(e.Message)),
            DbUpdateConcurrencyException e => new ConflictResult(),
            _ => new ObjectResult("Something went wrong...")
            {
                StatusCode = StatusCodes.Status500InternalServerError
            }
        };

        await result.ExecuteResultAsync(new()
        {
            HttpContext = context
        });
    }
}