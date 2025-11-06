using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using RabbitMQ.Client;

namespace OrderProcessing.Application.Messaging;

/// <summary>
/// RabbitMQ implementation of the message publisher
/// Application layer service for publishing messages to RabbitMQ
/// </summary>
public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConnection _connection;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IConnection connection, ILogger<RabbitMqPublisher> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default)
    {
        try
        {
            using var channel = _connection.CreateModel();
            
            // Declare the queue (idempotent operation)
            channel.QueueDeclare(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Serialize the message to JSON
            var messageJson = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(messageJson);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;

            // Publish the message
            channel.BasicPublish(
                exchange: string.Empty,
                routingKey: queueName,
                basicProperties: properties,
                body: body);

            _logger.LogInformation("Published message to queue {QueueName}: {Message}", queueName, messageJson);
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing message to queue {QueueName}", queueName);
            throw;
        }
    }
}
