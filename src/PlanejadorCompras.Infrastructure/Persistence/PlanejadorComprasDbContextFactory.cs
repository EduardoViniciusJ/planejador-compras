using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.Infrastructure;

public sealed class PlanejadorComprasDbContextFactory : IDesignTimeDbContextFactory<PlanejadorComprasDbContext>
{
    private const string ApiUserSecretsId = "38dbe4cc-4498-4a3f-b8e9-8fa7885d671c";

    public PlanejadorComprasDbContext CreateDbContext(string[] args)
    {
        var apiProjectPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "..", "PlanejadorCompras.API"));
        var userSecretsPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft",
            "UserSecrets",
            ApiUserSecretsId,
            "secrets.json");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddJsonFile(userSecretsPath, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'ConnectionStrings:DefaultConnection' was not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<PlanejadorComprasDbContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new PlanejadorComprasDbContext(optionsBuilder.Options);
    }
}
