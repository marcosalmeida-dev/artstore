using ArtStore.Application.Hubs;
using ArtStore.Shared.DTOs.Order.Events;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace ArtStore.Application.Common.EventHandlers;

public class OrderCreatedEventHandler : IDomainEventHandler
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;
    private readonly IHubContext<OrderManagementHub> _hubContext;

    public OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger,
        IHubContext<OrderManagementHub> hubContext)
    {
        _logger = logger;
        _hubContext = hubContext;
    }

    public bool CanHandle(object domainEvent)
    {
        return domainEvent is OrderCreatedEvent;
    }

    public async Task HandleAsync(object domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent is not OrderCreatedEvent orderCreatedEvent)
        {
            return;
        }

        _logger.LogInformation(
            "Order created: Id={OrderId}, Number={OrderNumber}, Email={CustomerEmail}, Amount={TotalAmount}, Date={OrderDate}",
            orderCreatedEvent.OrderId,
            orderCreatedEvent.OrderNumber,
            orderCreatedEvent.CustomerEmail,
            orderCreatedEvent.TotalAmount,
            orderCreatedEvent.OrderDate
        );

        // Notify clients via SignalR
        await _hubContext.Clients.All.SendAsync(IOrderManagementHub.OrderCreated, orderCreatedEvent, cancellationToken);

        await Task.CompletedTask;
    }
}