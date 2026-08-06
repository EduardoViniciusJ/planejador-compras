using Microsoft.AspNetCore.HttpOverrides;
using PlanejadorCompras.API.Extensions;

namespace PlanejadorCompras.API.Configuration;

internal static class ApiApplicationPipeline
{
    internal static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseForwardedHeaders();
        app.UseApiExceptionHandler();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseApiSecurityHeaders();
        app.UseCors(ApiCorsConfiguration.FrontendPolicy);
        app.UseRateLimiter();
        app.UseCookieRequestProtection();
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
