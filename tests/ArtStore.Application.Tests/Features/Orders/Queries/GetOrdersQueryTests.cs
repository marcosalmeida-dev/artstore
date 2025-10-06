using ArtStore.Application.Features.Order.Queries;
using ArtStore.Application.Tests.Fixtures;
using ArtStore.Domain.Entities;
using ArtStore.Shared.Models.Enums;
using FluentAssertions;

namespace ArtStore.Application.Tests.Features.Orders.Queries;

public class GetOrdersQueryTests : TestBase
{
    public GetOrdersQueryTests(ApplicationDbContextFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task Handle_GetAllOrders_ShouldReturnAllOrders()
    {
        // Arrange
        var context = GetDbContext();

        // Seed test orders
        var order1 = new Domain.Entities.Order
        {
            OrderNumber = "ORD-001",
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            CustomerName = "John Doe",
            CustomerEmail = "john@example.com",
            TotalAmount = 150m,
            PaymentMethod = PaymentMethodType.CreditCard,
            OrderSource = "Online",
            TenantId = 1
        };

        var order2 = new Domain.Entities.Order
        {
            OrderNumber = "ORD-002",
            OrderDate = DateTime.UtcNow.AddHours(-2),
            Status = OrderStatus.Processing,
            CustomerName = "Jane Smith",
            CustomerEmail = "jane@example.com",
            TotalAmount = 250m,
            PaymentMethod = PaymentMethodType.Cash,
            OrderSource = "POS",
            TenantId = 1
        };

        context.Orders.AddRange(order1, order2);
        await context.SaveChangesAsync();

        var handler = new GetAllOrdersQueryHandler(context);
        var query = new GetOrdersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Should().Contain(o => o.OrderNumber == "ORD-001");
        result.Should().Contain(o => o.OrderNumber == "ORD-002");
    }

    [Fact]
    public async Task Handle_GetAllOrders_ShouldOrderByOrderDateDescending()
    {
        // Arrange
        var context = GetDbContext();

        // Seed orders with different dates
        var olderOrder = new Domain.Entities.Order
        {
            OrderNumber = "ORD-OLD",
            OrderDate = DateTime.UtcNow.AddDays(-5),
            Status = OrderStatus.Delivered,
            CustomerName = "Old Customer",
            CustomerEmail = "old@example.com",
            TotalAmount = 100m,
            PaymentMethod = PaymentMethodType.Cash,
            OrderSource = "WalkIn",
            TenantId = 1
        };

        var newerOrder = new Domain.Entities.Order
        {
            OrderNumber = "ORD-NEW",
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            CustomerName = "New Customer",
            CustomerEmail = "new@example.com",
            TotalAmount = 200m,
            PaymentMethod = PaymentMethodType.CreditCard,
            OrderSource = "Online",
            TenantId = 1
        };

        context.Orders.AddRange(olderOrder, newerOrder);
        await context.SaveChangesAsync();

        var handler = new GetAllOrdersQueryHandler(context);
        var query = new GetOrdersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var orders = result.ToList();
        orders.First().OrderNumber.Should().Be("ORD-NEW"); // Most recent first
    }

    [Fact]
    public async Task Handle_GetAllOrders_ShouldIncludeOrderDetails()
    {
        // Arrange
        var context = GetDbContext();

        var order = new Domain.Entities.Order
        {
            OrderNumber = "ORD-DETAILS",
            OrderDate = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            CustomerName = "Test Customer",
            CustomerEmail = "test@example.com",
            TotalAmount = 500m,
            PaymentMethod = PaymentMethodType.CreditCard,
            OrderSource = "Online",
            TenantId = 1
        };

        var orderDetail = new OrderDetail
        {
            Order = order,
            ProductId = 1,
            Quantity = 2,
            UnitPrice = 250m
        };

        order.OrderDetails = new List<OrderDetail> { orderDetail };
        context.Orders.Add(order);
        await context.SaveChangesAsync();

        var handler = new GetAllOrdersQueryHandler(context);
        var query = new GetOrdersQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var orderWithDetails = result.FirstOrDefault(o => o.OrderNumber == "ORD-DETAILS");
        orderWithDetails.Should().NotBeNull();
        orderWithDetails!.OrderDetails.Should().HaveCount(1);
        orderWithDetails.OrderDetails!.First().Quantity.Should().Be(2);
        orderWithDetails.OrderDetails!.First().UnitPrice.Should().Be(250m);
    }
}
