using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrderProcessing.Application.Contracts.Orders;
using OrderProcessing.Application.Interfaces;
using OrderProcessing.Application.Services;
using OrderProcessing.Core.Entities;
using OrderProcessing.Core.Messages;
using OrderProcessing.DataAccess.Repositories;

namespace OrderProcessing.Application.Tests;

public class OrderServiceTests
{
    [Fact]
    public async Task CreateOrderAsync_ShouldCalculateTotalAndPublishMessage()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<OrderService>>();
        var repositoryMock = new Mock<IOrderRepository>();
        var publisherMock = new Mock<IMessagePublisher>();

        var service = new OrderService(loggerMock.Object, repositoryMock.Object, publisherMock.Object);

        var request = new CreateOrderRequest
        {
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            Items = new List<OrderItemRequest>
            {
                new OrderItemRequest { ProductName = "Product A", Quantity = 2, UnitPrice = 10.50m },
                new OrderItemRequest { ProductName = "Product B", Quantity = 1, UnitPrice = 25.00m }
            }
        };

        // Act
        var result = await service.CreateOrderAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.CustomerName.Should().Be("John Doe");
        result.CustomerEmail.Should().Be("john@example.com");
        result.TotalAmount.Should().Be(46.00m); // (2 * 10.50) + (1 * 25.00)
        result.Status.Should().Be("Pending");
        result.Items.Should().HaveCount(2);
        result.Items.First().ProductName.Should().Be("Product A");
        result.Items.First().TotalPrice.Should().Be(21.00m);

        // Verify message was published
        publisherMock.Verify(
            p => p.PublishAsync(
                "orders",
                It.Is<OrderCreatedMessage>(m =>
                    m.CustomerName == "John Doe" &&
                    m.TotalAmount == 46.00m &&
                    m.Items.Count == 2),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task GetOrderByIdAsync_WhenOrderExists_ShouldReturnMappedResponse()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<OrderService>>();
        var repositoryMock = new Mock<IOrderRepository>();
        var publisherMock = new Mock<IMessagePublisher>();

        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            CustomerName = "Jane Smith",
            CustomerEmail = "jane@example.com",
            Status = "Completed",
            TotalAmount = 100.00m,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItem>
            {
                new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductName = "Product X",
                    Quantity = 5,
                    UnitPrice = 20.00m,
                    TotalPrice = 100.00m
                }
            }
        };

        repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var service = new OrderService(loggerMock.Object, repositoryMock.Object, publisherMock.Object);

        // Act
        var result = await service.GetOrderByIdAsync(orderId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(orderId);
        result.CustomerName.Should().Be("Jane Smith");
        result.CustomerEmail.Should().Be("jane@example.com");
        result.Status.Should().Be("Completed");
        result.TotalAmount.Should().Be(100.00m);
        result.Items.Should().HaveCount(1);
        result.Items.First().ProductName.Should().Be("Product X");
        result.Items.First().Quantity.Should().Be(5);
        result.Items.First().UnitPrice.Should().Be(20.00m);

        repositoryMock.Verify(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class OrderMessageHandlerTests
{
    [Fact]
    public async Task HandleAsync_ShouldMapMessageToOrderAndSaveToDatabase()
    {
        // Arrange
        var repositoryMock = new Mock<IOrderRepository>();
        var loggerMock = new Mock<ILogger<OrderMessageHandler>>();

        var handler = new OrderMessageHandler(repositoryMock.Object, loggerMock.Object);

        var orderId = Guid.NewGuid();
        var message = new OrderCreatedMessage
        {
            OrderId = orderId,
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            TotalAmount = 150.00m,
            CreatedAt = DateTime.UtcNow,
            Items = new List<OrderItemMessage>
            {
                new OrderItemMessage { ProductName = "Product 1", Quantity = 3, UnitPrice = 50.00m }
            }
        };

        Order? capturedOrder = null;
        repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((order, _) => capturedOrder = order)
            .ReturnsAsync((Order order, CancellationToken _) => order);

        // Act
        await handler.HandleAsync(message, CancellationToken.None);

        // Assert
        repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Once);

        capturedOrder.Should().NotBeNull();
        capturedOrder!.Id.Should().Be(orderId);
        capturedOrder.CustomerName.Should().Be("Test Customer");
        capturedOrder.CustomerEmail.Should().Be("test@example.com");
        capturedOrder.TotalAmount.Should().Be(150.00m);
        capturedOrder.Status.Should().Be("Pending");
        capturedOrder.Items.Should().HaveCount(1);
        capturedOrder.Items.First().ProductName.Should().Be("Product 1");
        capturedOrder.Items.First().Quantity.Should().Be(3);
        capturedOrder.Items.First().UnitPrice.Should().Be(50.00m);
        capturedOrder.Items.First().TotalPrice.Should().Be(150.00m);
    }
}
