using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Core.Entities;
using OrderProcessing.Core.Messages;
using OrderProcessing.DataAccess.Repositories;

namespace OrderProcessing.Application.Services;

/// <summary>
/// Handler for processing order created messages
/// Handles message-to-entity mapping and persistence
/// </summary>
public class OrderMessageHandler : IOrderMessageHandler
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderMessageHandler> _logger;

    public OrderMessageHandler(
        IOrderRepository orderRepository,
        ILogger<OrderMessageHandler> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Processing order {OrderId} for customer {CustomerName}, Total: {TotalAmount:C}",
            message.OrderId,
            message.CustomerName,
            message.TotalAmount);

        // Map message to Order entity
        var order = new Order
        {
            Id = message.OrderId,
            CustomerName = message.CustomerName,
            CustomerEmail = message.CustomerEmail,
            TotalAmount = message.TotalAmount,
            Status = OrderStatus.Completed.ToString(),
            CreatedAt = message.CreatedAt,
            Items = message.Items.Select(i => new OrderItem
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList()
        };

        // Save to database
        await _orderRepository.CreateAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} saved to database successfully", message.OrderId);
    }
}
