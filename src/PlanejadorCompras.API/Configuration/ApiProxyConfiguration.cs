using System.Net;
using Microsoft.AspNetCore.HttpOverrides;

namespace PlanejadorCompras.API.Configuration;

internal static class ApiProxyConfiguration
{
    internal static IServiceCollection AddApiProxySupport(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                | ForwardedHeaders.XForwardedProto;
            options.ForwardLimit = 1;

            foreach (var value in configuration.GetSection("Proxy:KnownProxies").Get<string[]>()
                         ?? Array.Empty<string>())
            {
                if (!IPAddress.TryParse(value, out var address))
                {
                    throw new InvalidOperationException(
                        $"Invalid IP address in 'Proxy:KnownProxies': '{value}'.");
                }

                options.KnownProxies.Add(address);
            }
        });

        return services;
    }
}
