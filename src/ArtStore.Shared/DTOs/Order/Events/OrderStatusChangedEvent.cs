using ArtStore.Shared.Events.Base;
using ArtStore.Shared.Models.Enums;

namespace ArtStore.Shared.DTOs.Order.Events;

public class OrderStatusChangedEvent : BaseDomainEvent
{
    public OrderStatusChangedEvent(int orderId, string orderNumber, OrderStatus oldStatus, OrderStatus newStatus, DateTime changedAt, string? notes = null)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
        Notes = notes;
    }
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string? Notes { get; set; }
}