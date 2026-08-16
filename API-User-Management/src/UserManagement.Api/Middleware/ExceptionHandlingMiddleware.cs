using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using UserManagement.Application.Exceptions;

namespace UserManagement.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "An unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            NotFoundException =>
                new ErrorResponse(
                    HttpStatusCode.NotFound,
                    exception.Message),

            ValidationException =>
                new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    exception.Message),

            ArgumentException =>
                new ErrorResponse(
                    HttpStatusCode.BadRequest,
                    exception.Message),

            _ =>
                new ErrorResponse(
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred.")
        };

        context.Response.StatusCode = (int)response.StatusCode;

        var json = JsonSerializer.Serialize(
            new
            {
                statusCode = (int)response.StatusCode,
                message = response.Message
            });

        await context.Response.WriteAsync(json);
    }

    private sealed record ErrorResponse(
        HttpStatusCode StatusCode,
        string Message);
}