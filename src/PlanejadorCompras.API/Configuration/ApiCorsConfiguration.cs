namespace PlanejadorCompras.API.Configuration;

internal static class ApiCorsConfiguration
{
    internal const string FrontendPolicy = "Frontend";

    internal static IServiceCollection AddApiCors(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var allowedOrigins = configuration
            .GetSection("Cors:AllowedOrigins")
            .GetChildren()
            .Select(origin => origin.Value)
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin!)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(FrontendPolicy, policy =>
            {
                if (allowedOrigins.Length == 0)
                {
                    return;
                }

                policy.WithOrigins(allowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithExposedHeaders("Content-Disposition")
                    .AllowCredentials();
            });
        });

        return services;
    }
}
