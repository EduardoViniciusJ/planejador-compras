using Microsoft.AspNetCore.Mvc;

namespace PlanejadorCompras.API.ErrorHandling;

internal static class ApiProblemDetailsWriter
{
    internal static Task WriteAsync(
        HttpContext context,
        int statusCode,
        string title,
        string errorCode,
        CancellationToken cancellationToken = default)
    {
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

        return context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
}
