using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlanejadorCompras.Application.Exceptions;

namespace PlanejadorCompras.API.Extensions;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails();
        return services;
    }

    public static WebApplication UseApiExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                var (statusCode, title, errorCode) = MapException(exception);

                context.Response.StatusCode = statusCode;
                context.Response.ContentType = "application/problem+json";

                var problemDetails = new ProblemDetails
                {
                    Status = statusCode,
                    Title = title,
                    Instance = context.Request.Path
                };

                problemDetails.Extensions["errorCode"] = errorCode;
                problemDetails.Extensions["traceId"] = context.TraceIdentifier;

                await context.Response.WriteAsJsonAsync(problemDetails);
            });
        });

        return app;
    }

    private static (int StatusCode, string Title, string ErrorCode) MapException(Exception? exception)
    {
        return exception switch
        {
            AppException appException => (
                (int)appException.StatusCode,
                appException.Message,
                appException.ErrorCode),
            ArgumentNullException => (
                StatusCodes.Status400BadRequest,
                "Invalid request.",
                "bad_request"),
            ArgumentOutOfRangeException => (
                StatusCodes.Status400BadRequest,
                "Invalid request.",
                "bad_request"),
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid request.",
                "bad_request"),
            null => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "internal_server_error"),
            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                "internal_server_error")
        };
    }
}
