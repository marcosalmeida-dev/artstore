using ArtStore.Shared.Models.Enums;

namespace ArtStore.Domain.Entities;

public class OrderStatusHistory : BaseEntity
{
    public int OrderId { get; set; }
    public OrderStatus? OldStatus { get; set; }
    public OrderStatus NewStatus { get; set; }
    public DateTime ChangedAt { get; set; }
    public string ChangedBy { get; set; } = null!;
    public string? Notes { get; set; }

    // Navigation property
    public virtual Order Order { get; set; } = null!;
}