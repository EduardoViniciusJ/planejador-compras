using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanejadorCompras.Application.Services.Interfaces;
using PlanejadorCompras.Domain.Repositories.User;
using PlanejadorCompras.Infrastructure.Persistence;
using PlanejadorCompras.Infrastructure.Repositories;
using PlanejadorCompras.Infrastructure.Services;

namespace PlanejadorCompras.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContextSqlServer(services, configuration);
        AddRepositories(services);
        AddExternalServices(services);

        return services;
    }

    private static void AddDbContextSqlServer(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing connection string 'ConnectionStrings:DefaultConnection'.");
        }

        services.AddDbContext<PlanejadorComprasDbContext>(options =>
            options.UseSqlServer(connectionString));
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddExternalServices(IServiceCollection services)
    {
        services.AddScoped<IGoogleTokenValidator, GoogleTokenValidator>();
    }
}
