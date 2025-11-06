using OrderProcessing.Application.Contracts.Orders;

namespace OrderProcessing.Application.Interfaces;

/// <summary>
/// Service interface for order business logic
/// </summary>
public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
    Task<bool> UpdateOrderStatusAsync(Guid orderId, string status, CancellationToken cancellationToken = default);
}
