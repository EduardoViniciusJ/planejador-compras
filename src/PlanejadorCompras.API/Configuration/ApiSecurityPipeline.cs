using PlanejadorCompras.API.ErrorHandling;
using PlanejadorCompras.API.Security;

namespace PlanejadorCompras.API.Configuration;

internal static class ApiSecurityPipeline
{
    internal static IApplicationBuilder UseApiSecurityHeaders(
        this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
            context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
            context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
            context.Response.Headers.TryAdd(
                "Permissions-Policy",
                "camera=(), microphone=(), geolocation=()");

            if (context.Request.Path.StartsWithSegments("/api/auth"))
            {
                context.Response.Headers["Cache-Control"] = "no-store";
                context.Response.Headers["Pragma"] = "no-cache";
            }

            await next();
        });

    internal static IApplicationBuilder UseCookieRequestProtection(
        this IApplicationBuilder app) =>
        app.Use(async (context, next) =>
        {
            var hasAccessTokenCookie = context.Request.Cookies.ContainsKey(
                AuthenticationConstants.AccessTokenCookieName);

            if (hasAccessTokenCookie
                && IsUnsafeHttpMethod(context.Request.Method)
                && !HasXmlHttpRequestHeader(context))
            {
                await ApiProblemDetailsWriter.WriteAsync(
                    context,
                    StatusCodes.Status401Unauthorized,
                    "Missing required request header.",
                    "missing_x_requested_with",
                    context.RequestAborted);
                return;
            }

            await next();
        });

    private static bool IsUnsafeHttpMethod(string method) =>
        HttpMethods.IsPost(method)
        || HttpMethods.IsPut(method)
        || HttpMethods.IsPatch(method)
        || HttpMethods.IsDelete(method);

    private static bool HasXmlHttpRequestHeader(HttpContext context) =>
        string.Equals(
            context.Request.Headers[AuthenticationConstants.XmlHttpRequestHeaderName],
            AuthenticationConstants.XmlHttpRequestHeaderValue,
            StringComparison.OrdinalIgnoreCase);
}
