using ArtStore.Shared.Interfaces.Command;
using ArtStore.Shared.Models.Enums;

namespace ArtStore.Shared.DTOs.Order.Commands;

public class CreateOrderCommand : ICommand<Result<string>>
{
    public string OrderSource { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethodType PaymentMethod { get; set; } = PaymentMethodType.None;
    public decimal TotalAmount { get; set; }
    public string? CustomerId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? CouponCode { get; set; }
    public decimal? CouponDiscount { get; set; }
    public List<OrderDetailDto>? OrderDetails { get; set; }
}