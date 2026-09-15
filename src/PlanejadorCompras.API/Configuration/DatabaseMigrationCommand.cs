using Microsoft.EntityFrameworkCore;
using Npgsql;
using PlanejadorCompras.Infrastructure.Persistence;

namespace PlanejadorCompras.API.Configuration;

internal static class DatabaseMigrationCommand
{
    // Stable session lock shared by every deployment of this application.
    private const long LockId = 734610298412L;

    internal static async Task<int> RunAsync()
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            Console.Error.WriteLine("Migrations: configure ConnectionStrings__DefaultConnection.");
            return 1;
        }

        try
        {
            // Keep a dedicated lock session alive until migrations finish. EF/Npgsql
            // must manage its own connection when installing extensions such as citext.
            // Pooling is disabled so disposal always releases the session lock.
            var settings = new NpgsqlConnectionStringBuilder(connectionString) { Pooling = false };
            await using var connection = new NpgsqlConnection(settings.ConnectionString);
            await connection.OpenAsync();
            Console.WriteLine("Migrations: waiting for the database deployment lock (up to 5 minutes).");
            await using var command = new NpgsqlCommand("SELECT pg_advisory_lock(@key)", connection)
            {
                CommandTimeout = 300
            };
            command.Parameters.AddWithValue("key", LockId);
            await command.ExecuteNonQueryAsync();

            var options = new DbContextOptionsBuilder<PlanejadorComprasDbContext>()
                .UseNpgsql(settings.ConnectionString)
                .Options;
            await using var context = new PlanejadorComprasDbContext(options);
            var pending = (await context.Database.GetPendingMigrationsAsync()).ToArray();
            Console.WriteLine($"Migrations: {pending.Length} pending.");
            await context.Database.MigrateAsync();
            Console.WriteLine("Migrations: completed successfully.");
            return 0;
        }
        catch (Exception exception)
        {
            // Do not log connection strings, SQL data or secrets in deployment output.
            Console.Error.WriteLine($"Migrations failed ({exception.GetType().Name}). API startup cancelled.");
            if (exception is PostgresException postgres)
                Console.Error.WriteLine($"PostgreSQL SQLSTATE: {postgres.SqlState}");
            return 1;
        }
    }
}
