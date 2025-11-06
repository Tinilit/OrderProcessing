using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Core.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessing.Worker;

/// <summary>
/// Background service that consumes order messages from RabbitMQ
/// Delegates message processing to the Application layer
/// </summary>
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private IModel? _channel;

    public Worker(
        ILogger<Worker> logger,
        IConnection connection,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _connection = connection;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Worker starting...");

        // Create a channel
        _channel = _connection.CreateModel();

        // Declare the queue
        _channel.QueueDeclare(
            queue: "orders",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        _logger.LogInformation("Worker connected to RabbitMQ and listening to 'orders' queue");

        // Create a consumer
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<OrderCreatedMessage>(messageJson);

                if (message != null)
                {
                    // Create a scope for scoped services (proper DI)
                    using var scope = _serviceScopeFactory.CreateScope();
                    var handler = scope.ServiceProvider.GetRequiredService<IOrderMessageHandler>();

                    // Delegate to Application layer handler
                    await handler.HandleAsync(message, stoppingToken);

                    // Acknowledge the message
                    _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                // Reject the message and requeue it
                _channel?.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        // Set QoS to process one message at a time
        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        // Start consuming
        _channel.BasicConsume(
            queue: "orders",
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation("Worker is now consuming messages from 'orders' queue");

        // Keep the worker running
        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopping...");
        
        if (_channel != null)
        {
            _channel.Close();
            _channel.Dispose();
        }

        return base.StopAsync(cancellationToken);
    }
}
