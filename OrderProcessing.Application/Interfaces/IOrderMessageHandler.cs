using OrderProcessing.Core.Messages;

namespace OrderProcessing.Application.Interfaces;

/// <summary>
/// Handler for processing order created messages
/// Application layer abstraction for message-driven order processing
/// </summary>
public interface IOrderMessageHandler
{
    /// <summary>
    /// Handles an order created message and persists the order
    /// </summary>
    /// <param name="message">The order created message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task HandleAsync(OrderCreatedMessage message, CancellationToken cancellationToken);
}
