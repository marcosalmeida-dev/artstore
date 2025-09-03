using ArtStore.Shared.Events.Base;

namespace ArtStore.Shared.DTOs.Order.Events;

public class OrderCreatedEvent : BaseDomainEvent
{
    public OrderCreatedEvent(long orderId, string orderNumber, string customerEmail, decimal totalAmount, DateTime orderDate)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        CustomerEmail = customerEmail;
        TotalAmount = totalAmount;
        OrderDate = orderDate;
    }
    public long OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
}