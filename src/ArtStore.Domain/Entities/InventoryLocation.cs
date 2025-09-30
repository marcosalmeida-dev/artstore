// src/ArtStore.Domain/Entities/InventoryLocation.cs
using ArtStore.Domain.Common.Entities;

namespace ArtStore.Domain.Entities;

/// <summary>
/// Represents a physical or logical location where inventory is stored (warehouse, store, etc.).
/// </summary>
public class InventoryLocation : BaseTenantEntity<int>
{
    /// <summary>
    /// Name of the location (required, max 128 characters).
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Optional code for the location (max 32 characters, unique per tenant).
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Whether this is the default location for the tenant.
    /// </summary>
    public bool IsDefault { get; set; } = true;

    // Navigation properties
    /// <summary>
    /// Collection of inventory items at this location.
    /// </summary>
    public virtual ICollection<InventoryItem> InventoryItems { get; set; } = new List<InventoryItem>();
}