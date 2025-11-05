using OrderProcessing.Core.DTO.Orders;

namespace OrderProcessing.Core.Interfaces;

/// <summary>
/// Service interface for order business logic
/// </summary>
public interface IOrderService
{
    Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderResponse?> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<OrderResponse>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
}
