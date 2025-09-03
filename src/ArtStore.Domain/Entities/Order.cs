using ArtStore.Shared.Models.Enums;

namespace ArtStore.Domain.Entities;

public class Order : BaseTenantEntity<long>
{
    public string OrderSource { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CustomerId { get; set; } // Nullable for guest orders
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Navigation properties
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<OrderStatusHistory> OrderStatusHistories { get; set; } = new List<OrderStatusHistory>();
}
