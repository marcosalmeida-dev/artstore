using ArtStore.Shared.Models.Enums;

namespace ArtStore.Shared.DTOs.Order;

public class OrderStatusHistoryDto
{
    public long Id { get; set; }
    public long OrderId { get; set; }
    public OrderStatus? OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; } = null!;
    public string? Notes { get; set; }
}