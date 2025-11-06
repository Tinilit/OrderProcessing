using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OrderProcessing.DataAccess.Repositories;

namespace OrderProcessing.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the OrderProcessing data access services (Database + RabbitMQ connection) to the service collection.
    /// </summary>
    public static IServiceCollection AddOrderProcessingInfrastructure(
        this IServiceCollection services,
        IHostApplicationBuilder builder)
    {
        // Add DbContext with PostgreSQL
        builder.AddNpgsqlDbContext<OrderProcessingDbContext>("orderdb");

        // Register repositories
        services.AddScoped<IOrderRepository, OrderRepository>();

        // Register RabbitMQ connection (used by Application layer's RabbitMqPublisher)
        builder.AddRabbitMQClient("messaging");

        return services;
    }
}
