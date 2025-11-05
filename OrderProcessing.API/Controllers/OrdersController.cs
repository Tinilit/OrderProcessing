using Microsoft.AspNetCore.Mvc;
using OrderProcessing.Core.DTO.Orders;
using OrderProcessing.Core.Interfaces;

namespace OrderProcessing.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new order
    /// </summary>
    /// <param name="request">Order creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order response with status</returns>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponse>> CreateOrder(
        [FromBody] CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating order for customer: {CustomerName}", request.CustomerName);

            // Model validation is handled automatically by [ApiController] attribute
            // If validation fails, it returns 400 Bad Request automatically

            var response = await _orderService.CreateOrderAsync(request, cancellationToken);

            _logger.LogInformation("Order created successfully with status: {Status}", response.Status);

            return CreatedAtAction(
                nameof(GetOrderById),
                new { id = Guid.NewGuid() }, // Placeholder until we add OrderId to response
                response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order for customer: {CustomerName}", request.CustomerName);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the order");
        }
    }

    /// <summary>
    /// Gets an order by ID
    /// </summary>
    /// <param name="id">Order ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Order details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrderById(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var order = await _orderService.GetOrderByIdAsync(id, cancellationToken);

            if (order == null)
            {
                return NotFound($"Order with ID {id} not found");
            }

            return Ok(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving order: {OrderId}", id);
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the order");
        }
    }

    /// <summary>
    /// Gets all orders
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of orders</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderResponse>>> GetAllOrders(CancellationToken cancellationToken)
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync(cancellationToken);
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all orders");
            return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving orders");
        }
    }
}
