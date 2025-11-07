using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OrderProcessing.DataAccess.Extensions;

/// <summary>
/// Extension methods for automatic database initialization
/// </summary>
public static class DatabaseInitializationExtensions
{
    /// <summary>
    /// Ensures the database is created and all migrations are applied
    /// This is called automatically on application startup
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<OrderProcessingDbContext>>();

        try
        {
            var context = services.GetRequiredService<OrderProcessingDbContext>();
            
            logger.LogInformation("Checking database migrations...");

            // Apply any pending migrations
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migration(s)...", pendingMigrations.Count());
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully");
            }
            else
            {
                logger.LogInformation("Database is up to date");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
    }
}
