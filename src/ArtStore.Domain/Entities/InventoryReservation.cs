// src/ArtStore.Domain/Entities/InventoryReservation.cs
using ArtStore.Domain.Common.Entities;
using ArtStore.Domain.Entities.Enums;

namespace ArtStore.Domain.Entities;

/// <summary>
/// Represents a soft hold on inventory tied to an order.
/// Reservations have a TTL and can be committed (hard deduction) or released.
/// </summary>
public class InventoryReservation : BaseTenantEntity<long>
{
    /// <summary>
    /// Product being reserved.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Location where the product is reserved.
    /// </summary>
    public int InventoryLocationId { get; set; }

    /// <summary>
    /// Order this reservation belongs to.
    /// </summary>
    public long OrderId { get; set; }

    /// <summary>
    /// Specific order detail line item.
    /// </summary>
    public long OrderDetailId { get; set; }

    /// <summary>
    /// Quantity reserved.
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// When the reservation was created (defaults to UtcNow).
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Optional expiration time for the reservation (TTL).
    /// </summary>
    public DateTimeOffset? ExpiresAt { get; set; }

    /// <summary>
    /// Current status of the reservation (defaults to Active).
    /// </summary>
    public ReservationStatus Status { get; set; } = ReservationStatus.Active;

    // Navigation properties
    /// <summary>
    /// The product entity.
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// The location entity.
    /// </summary>
    public virtual InventoryLocation Location { get; set; } = null!;
}