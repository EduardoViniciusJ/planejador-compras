using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Missing connection string 'ConnectionStrings:DefaultConnection'.");
        }

        services.AddDbContext<PlanejadorComprasDbContext>(options =>
            options.UseSqlServer(connectionString));

        return services;
    }
}
