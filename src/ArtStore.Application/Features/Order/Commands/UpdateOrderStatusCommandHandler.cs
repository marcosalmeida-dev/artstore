using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Order.Commands;
using ArtStore.Shared.DTOs.Order.Events;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Application.Features.Order.Commands;

public class UpdateOrderStatusCommandHandler : ICommandHandler<UpdateOrderStatusCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateOrderStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return await Result.FailureAsync("Order not found.");
        }

        // Capture the old status before making changes
        var oldStatus = order.Status;

        order.Status = request.Status;
        order.Notes = request.Notes ?? order.Notes;
        order.LastModified = DateTime.UtcNow;

        order.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OldStatus = oldStatus,
            NewStatus = request.Status,
            ChangedAt = order.LastModified.Value,
            ChangedBy = "System" // This should ideally be the user making the change, if available
        });

        // Add domain event for order status change
        order.AddDomainEvent(new OrderStatusChangedEvent(
            order.Id,
            order.OrderNumber,
            oldStatus,
            request.Status,
            order.LastModified.Value,
            order.Notes));

        await _context.SaveChangesAsync(cancellationToken);

        return await Result.SuccessAsync();
    }
}