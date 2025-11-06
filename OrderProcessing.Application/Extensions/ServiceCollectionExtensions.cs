using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Messaging;
using OrderProcessing.Application.Services;

namespace OrderProcessing.Application.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds application services to the service collection
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IOrderService, OrderService>();
        
        // Register message handler
        services.AddScoped<IOrderMessageHandler, OrderMessageHandler>();
        
        // Register messaging services
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();

        return services;
    }
}
