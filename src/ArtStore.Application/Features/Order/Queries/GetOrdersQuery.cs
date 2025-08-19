using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Order;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Order.Queries;

public class GetOrdersQuery : IQuery<IEnumerable<OrderDto>>
{
}

public class GetAllOrdersQueryHandler : IQueryHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAllOrdersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Orders
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Product)
            .Include(o => o.OrderStatusHistories)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync(cancellationToken);

        return data.Select(order => new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderDate = order.OrderDate,
            Status = order.Status,
            CustomerName = order.CustomerName,
            TotalAmount = order.TotalAmount,
            PaymentMethod = order.PaymentMethod,
            OrderSource = order.OrderSource,
            OrderDetails = order.OrderDetails?.Select(detail => new OrderDetailDto
            {
                Id = detail.Id,
                ProductId = detail.ProductId,
                ProductName = detail.Product?.Name ?? string.Empty,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice
            }).ToList(),
            OrderStatusHistories = order.OrderStatusHistories?.Select(history => new OrderStatusHistoryDto
            {
                Id = history.Id,
                OrderId = history.OrderId,
                OldStatus = history.OldStatus,
                NewStatus = history.NewStatus,
                ChangedBy = history.ChangedBy,
                ChangedAt = history.ChangedAt
            }).ToList(),
        });
    }
}