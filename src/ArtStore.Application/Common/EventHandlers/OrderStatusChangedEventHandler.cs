using ArtStore.Shared.DTOs.Order.Events;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using ArtStore.Application.Hubs;

namespace ArtStore.Application.Common.EventHandlers;

public class OrderStatusChangedEventHandler : IDomainEventHandler
{
    private readonly ILogger<OrderStatusChangedEventHandler> _logger;
    private readonly IHubContext<OrderManagementHub> _hubContext;

    public OrderStatusChangedEventHandler(
        ILogger<OrderStatusChangedEventHandler> logger,
        IHubContext<OrderManagementHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }
        
    public bool CanHandle(object domainEvent)
    {
        return domainEvent is OrderStatusChangedEvent;
    }

    public async Task HandleAsync(object domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is not OrderStatusChangedEvent statusChangedEvent)
            return;

        _logger.LogInformation(
            "Order status changed: Id={OrderId}, OldStatus={OldStatus}, NewStatus={NewStatus}, Reason={Reason}",
            statusChangedEvent.OrderId,
            statusChangedEvent.OldStatus,
            statusChangedEvent.NewStatus,
            statusChangedEvent.Notes
        );

        // Notify clients via SignalR
        await _hubContext.Clients.All.SendAsync(IOrderManagementHub.OrderStatusChanged, statusChangedEvent, cancellationToken);

        await Task.CompletedTask;
    }
}