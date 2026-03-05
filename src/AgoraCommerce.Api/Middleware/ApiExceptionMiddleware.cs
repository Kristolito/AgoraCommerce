using System.Net;
using System.Text.Json;
using AgoraCommerce.Application.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Middleware;

public sealed class ApiExceptionMiddleware(RequestDelegate next, ILogger<ApiExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing request.");
            await HandleException(context, ex);
        }
    }

    private static async Task HandleException(HttpContext context, Exception exception)
    {
        var (statusCode, payload) = exception switch
        {
            ValidationException validationException => BuildValidationProblem(validationException),
            NotFoundException notFound => BuildProblem((int)HttpStatusCode.NotFound, "Not Found", notFound.Message),
            ForbiddenException forbidden => BuildProblem((int)HttpStatusCode.Forbidden, "Forbidden", forbidden.Message),
            ConflictException conflict => BuildProblem((int)HttpStatusCode.Conflict, "Conflict", conflict.Message),
            _ => BuildProblem((int)HttpStatusCode.InternalServerError, "Internal Server Error", "An unexpected error occurred.")
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }

    private static (int, object) BuildProblem(int statusCode, string title, string detail)
    {
        return (statusCode, new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail
        });
    }

    private static (int, object) BuildValidationProblem(ValidationException validationException)
    {
        var errors = validationException.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(x => x.ErrorMessage).Distinct().ToArray());

        var payload = new ValidationProblemDetails(errors)
        {
            Status = (int)HttpStatusCode.BadRequest,
            Title = "Validation Failed",
            Detail = "One or more validation errors occurred."
        };

        return ((int)HttpStatusCode.BadRequest, payload);
    }
}
