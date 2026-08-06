using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using PlanejadorCompras.API.ErrorHandling;

namespace PlanejadorCompras.API.Configuration;

internal static class ApiRateLimitingConfiguration
{
    internal const string LoginPolicy = "auth-login";
    internal const string EmailPolicy = "auth-email";

    internal static IServiceCollection AddApiRateLimiting(
        this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = (context, cancellationToken) =>
                new ValueTask(ApiProblemDetailsWriter.WriteAsync(
                    context.HttpContext,
                    StatusCodes.Status429TooManyRequests,
                    "Too many requests.",
                    "rate_limited",
                    cancellationToken));

            options.AddPolicy(LoginPolicy, httpContext =>
                CreateFixedWindowPartition(httpContext, 10, TimeSpan.FromMinutes(1)));
            options.AddPolicy(EmailPolicy, httpContext =>
                CreateFixedWindowPartition(httpContext, 5, TimeSpan.FromMinutes(10)));
        });

        return services;
    }

    private static RateLimitPartition<string> CreateFixedWindowPartition(
        HttpContext context,
        int permitLimit,
        TimeSpan window) =>
        RateLimitPartition.GetFixedWindowLimiter(
            GetClientPartitionKey(context),
            _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = permitLimit,
                QueueLimit = 0,
                Window = window
            });

    private static string GetClientPartitionKey(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString()
        ?? context.Request.Headers.UserAgent.ToString()
        ?? "unknown";
}
