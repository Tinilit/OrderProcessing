using System.Text;
using System.Text.Json;
using OrderProcessing.Core.Messages;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace OrderProcessing.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConnection _connection;
    private IModel? _channel;

    public Worker(ILogger<Worker> logger, IConnection connection)
    {
        _logger = logger;
        _connection = connection;
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
        consumer.Received += (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var messageJson = Encoding.UTF8.GetString(body);
                var message = JsonSerializer.Deserialize<OrderCreatedMessage>(messageJson);

                if (message != null)
                {
                    _logger.LogInformation(
                        "Processing order {OrderId} for customer {CustomerName}, Total: {TotalAmount:C}",
                        message.OrderId,
                        message.CustomerName,
                        message.TotalAmount);

                    // TODO: Implement actual order processing logic here
                    // For example: send confirmation email, update inventory, etc.
                    Thread.Sleep(1000); // Simulate processing

                    _logger.LogInformation("Order {OrderId} processed successfully", message.OrderId);

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
