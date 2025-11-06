namespace OrderProcessing.Application.Interfaces;

/// <summary>
/// Interface for publishing messages to a message broker
/// Application layer abstraction for asynchronous messaging
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publishes a message to the specified queue
    /// </summary>
    /// <typeparam name="T">The type of message to publish</typeparam>
    /// <param name="queueName">The name of the queue</param>
    /// <param name="message">The message to publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default);
}
