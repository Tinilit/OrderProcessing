using OrderProcessing.Core.Entities;

namespace OrderProcessing.Core.Interfaces;

/// <summary>
/// Repository interface for order data access
/// </summary>
public interface IOrderRepository
{
    Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
