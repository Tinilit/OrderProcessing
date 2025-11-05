using System.ComponentModel.DataAnnotations;

namespace OrderProcessing.Core.DTO.Orders;

/// <summary>
/// Request model for creating a new order
/// </summary>
public class CreateOrderRequest
{
    [Required(ErrorMessage = "Customer name is required")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Customer name must be between 2 and 100 characters")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Customer email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string CustomerEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "At least one item is required")]
    [MinLength(1, ErrorMessage = "Order must contain at least one item")]
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Product name must be between 1 and 200 characters")]
    public string ProductName { get; set; } = string.Empty;

    [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000")]
    public int Quantity { get; set; }

    [Range(0.01, 1000000, ErrorMessage = "Unit price must be between 0.01 and 1000000")]
    public decimal UnitPrice { get; set; }
}
