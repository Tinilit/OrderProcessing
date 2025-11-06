using Microsoft.Extensions.Logging;
using OrderProcessing.Application.Contracts.Orders;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Core.Entities;
using OrderProcessing.Core.Messages;
using OrderProcessing.DataAccess.Repositories;

namespace OrderProcessing.Application.Services;

/// <summary>
/// Application service for order operations
/// Handles business logic, validation, and orchestration
/// </summary>
public class OrderService : IOrderService
{
    private readonly ILogger<OrderService> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IMessagePublisher _messagePublisher;

    public OrderService(
        ILogger<OrderService> logger,
        IOrderRepository orderRepository,
        IMessagePublisher messagePublisher)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _messagePublisher = messagePublisher;
    }

    public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating order for customer: {CustomerName}", request.CustomerName);

        // 1. Calculate total amount and generate order ID
        var totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);
        var orderId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow;
        
        // 2. Publish message to queue (async processing by Worker)
        var message = new OrderCreatedMessage
        {
            OrderId = orderId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            TotalAmount = totalAmount,
            CreatedAt = createdAt,
            Items = request.Items.Select(i => new OrderItemMessage
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };

        await _messagePublisher.PublishAsync("orders", message, cancellationToken);

        _logger.LogInformation("Order message published to queue for customer: {CustomerName}", request.CustomerName);

        // 3. Return response (order will be persisted by Worker)
        return new OrderResponse
        {
            Id = orderId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            Status = OrderStatus.Pending.ToString(),
            TotalAmount = totalAmount,
            CreatedAt = createdAt,
            Items = request.Items.Select(i => new OrderItemResponse
            {
                Id = Guid.NewGuid(),
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList()
        };
    }

    public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving order: {OrderId}", orderId);

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", orderId);
            return null;
        }

        return MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Retrieving all orders");

        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return orders.Select(MapToResponse);
    }

    public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string status, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating order {OrderId} status to: {Status}", orderId, status);

        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);

        if (order == null)
        {
            _logger.LogWarning("Order not found: {OrderId}", orderId);
            return false;
        }

        order.Status = status;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        _logger.LogInformation("Order {OrderId} status updated to: {Status}", orderId, status);

        return true;
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            Status = order.Status,
            TotalAmount = order.TotalAmount,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = order.Items.Select(i => new OrderItemResponse
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.TotalPrice
            }).ToList()
        };
    }
}
