using ArtStore.Shared.Models.Enums;

namespace ArtStore.UI.Client.Models;

public class CreateOrderModel
{
    public string OrderNumber { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string? CustomerPhone { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public PaymentMethodType PaymentMethod { get; set; }
    public List<ProductModel> Items { get; set; } = new List<ProductModel>();
}