// src/ArtStore.Domain/Entities/StockMovement.cs
using ArtStore.Domain.Common.Entities;
using ArtStore.Domain.Entities.Enums;

namespace ArtStore.Domain.Entities;

/// <summary>
/// Immutable ledger entry recording all inventory movements.
/// These records are append-only and never modified for auditability.
/// </summary>
public class StockMovement : BaseTenantEntity<long>
{
    /// <summary>
    /// Optional reference to the inventory item (may be null for movements before item creation).
    /// </summary>
    public long? InventoryItemId { get; set; }

    /// <summary>
    /// Product involved in the movement.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Location where the movement occurred.
    /// </summary>
    public int InventoryLocationId { get; set; }

    /// <summary>
    /// Quantity moved (positive for increases, negative for decreases).
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Type of movement.
    /// </summary>
    public StockMovementType Type { get; set; }

    /// <summary>
    /// When the movement occurred (defaults to UtcNow).
    /// </summary>
    public DateTimeOffset OccurredAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Optional reference to the order (if related to an order).
    /// </summary>
    public long? OrderId { get; set; }

    /// <summary>
    /// Optional reference to the order detail (if related to a specific line item).
    /// </summary>
    public long? OrderDetailId { get; set; }

    /// <summary>
    /// Optional external reference (e.g., PO number, transfer reference).
    /// </summary>
    public string? Reference { get; set; }

    /// <summary>
    /// Optional notes explaining the movement.
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Optional reference to the inventory item entity.
    /// </summary>
    public virtual InventoryItem? InventoryItem { get; set; }

    /// <summary>
    /// The product entity.
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// The location entity.
    /// </summary>
    public virtual InventoryLocation Location { get; set; } = null!;
}