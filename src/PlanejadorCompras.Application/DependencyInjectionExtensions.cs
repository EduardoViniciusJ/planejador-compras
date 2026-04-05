using Microsoft.Extensions.DependencyInjection;
using PlanejadorCompras.Application.UseCases.Auth;

namespace PlanejadorCompras.Application;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        AddUseCases(services);

        return services;
    }

    private static void AddUseCases(IServiceCollection services)
    {
        services.AddScoped<GoogleLoginUseCase>();
    }
}
