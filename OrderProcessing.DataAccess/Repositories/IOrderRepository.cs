using OrderProcessing.Core.Entities;

namespace OrderProcessing.DataAccess.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
