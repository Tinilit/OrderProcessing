namespace OrderProcessing.Core.Messages;

/// <summary>
/// Message sent to the queue when an order is created
/// </summary>
public class OrderCreatedMessage
{
    public Guid OrderId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemMessage> Items { get; set; } = new();
}

public class OrderItemMessage
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
