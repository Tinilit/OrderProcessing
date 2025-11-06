using Microsoft.EntityFrameworkCore;
using OrderProcessing.Core.Entities;

namespace OrderProcessing.DataAccess.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly OrderProcessingDbContext _context;

    public OrderRepository(OrderProcessingDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        order.Id = Guid.NewGuid();
        order.CreatedAt = DateTime.UtcNow;
        
        foreach (var item in order.Items)
        {
            item.Id = Guid.NewGuid();
            item.OrderId = order.Id;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        
        return order;
    }

    public async Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        order.UpdatedAt = DateTime.UtcNow;
        
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
        
        return order;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
        
        if (order == null)
        {
            return false;
        }

        _context.Orders.Remove(order);
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}
