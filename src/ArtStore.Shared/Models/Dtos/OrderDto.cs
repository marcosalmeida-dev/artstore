using ArtStore.Shared.DTOs.Order;
using ArtStore.Shared.Models.Enums;

namespace ArtStore.Shared.Models.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int? TenantId { get; set; }
    public string OrderSource { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public decimal TotalAmount { get; set; }
    public string? CustomerId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }

    // Audit fields
    public DateTime? Created { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? LastModified { get; set; }
    public string? LastModifiedBy { get; set; }

    public List<OrderDetailDto> OrderDetails { get; set; } = new();
}