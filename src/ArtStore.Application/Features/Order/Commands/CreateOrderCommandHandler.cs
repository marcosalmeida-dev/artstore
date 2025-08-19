using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Order.Commands;
using ArtStore.Shared.DTOs.Order.Events;
using ArtStore.Shared.Interfaces.Command;
using ArtStore.Shared.Models.Enums;
using Microsoft.Extensions.Logging;

namespace ArtStore.Application.Features.Order.Commands;

public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, Result<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IApplicationDbContext context,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var order = new Domain.Entities.Order
            {
                OrderSource = request.OrderSource,
                OrderNumber = await GenerateOrderNumberAsync(),
                OrderDate = DateTime.UtcNow,
                PaymentMethod = request.PaymentMethod,
                //Consider only paid orders
                Status = OrderStatus.Confirmed,
                TotalAmount = request.TotalAmount,
                CustomerId = request.CustomerId,
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                CustomerPhone = request.CustomerPhone,
                ShippingAddress = request.ShippingAddress,
                Notes = request.Notes,
                Created = DateTime.UtcNow,
                CreatedBy = request.CustomerId ?? "Guest",
                LastModified = DateTime.UtcNow,
                LastModifiedBy = request.CustomerId ?? "Guest",
                OrderDetails = request.OrderDetails.Select(od => new OrderDetail
                {
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    TotalPrice = od.TotalPrice
                }).ToList(),
                OrderStatusHistories = new List<OrderStatusHistory>
                {
                    new OrderStatusHistory
                    {
                        OldStatus = OrderStatus.Confirmed,
                        NewStatus = OrderStatus.Confirmed,
                        ChangedAt = DateTime.UtcNow,
                        ChangedBy = request.CustomerId ?? "Guest"
                    }
                }
            };

            // Add domain event for order creation
            order.AddDomainEvent(new OrderCreatedEvent(
                order.Id,
                order.OrderNumber,
                order.CustomerEmail,
                order.TotalAmount,
                order.OrderDate));

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(cancellationToken);

            return await Result<string>.SuccessAsync(order.OrderNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order for customer {CustomerEmail}", request.CustomerEmail);
            return await Result<string>.FailureAsync("An error occurred while creating the order.");
        }
    }

    private async Task<string> GenerateOrderNumberAsync()
    {
        var today = DateTime.UtcNow.Date;

        var lastOrder = await _context.Orders
            .Where(o => o.OrderDate.Date == today)
            .OrderByDescending(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        string newOrderNumber;

        if (lastOrder == null)
        {
            newOrderNumber = "A001";
        }
        else
        {
            var lastOrderNumber = lastOrder.OrderNumber;
            var numericPart = int.Parse(lastOrderNumber.Substring(1));
            newOrderNumber = $"A{(numericPart + 1):D3}";
        }

        return newOrderNumber;
    }
}