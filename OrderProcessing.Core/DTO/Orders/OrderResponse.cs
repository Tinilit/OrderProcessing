namespace OrderProcessing.Core.DTO.Orders;

/// <summary>
/// Response model for order operations
/// </summary>
public class OrderResponse
{
    public string Status { get; set; } = OrderStatus.Pending.ToString();
}