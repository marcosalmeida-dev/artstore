// src/ArtStore.Domain/Entities/InventoryItem.cs
using ArtStore.Domain.Common.Entities;

namespace ArtStore.Domain.Entities;

/// <summary>
/// Represents the inventory snapshot for a specific product at a specific location.
/// Unique per (TenantId, ProductId, InventoryLocationId).
/// </summary>
public class InventoryItem : BaseTenantEntity<long>
{
    /// <summary>
    /// The product being tracked.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The location where this inventory is held.
    /// </summary>
    public int InventoryLocationId { get; set; }

    /// <summary>
    /// Current quantity on hand.
    /// </summary>
    public decimal OnHand { get; set; }

    /// <summary>
    /// Minimum safety stock level to maintain.
    /// </summary>
    public decimal SafetyStock { get; set; }

    /// <summary>
    /// Point at which to trigger reordering.
    /// </summary>
    public decimal ReorderPoint { get; set; }

    // Navigation properties
    /// <summary>
    /// The product entity.
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// The location entity.
    /// </summary>
    public virtual InventoryLocation Location { get; set; } = null!;

    /// <summary>
    /// Collection of stock movements for this item.
    /// </summary>
    public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}