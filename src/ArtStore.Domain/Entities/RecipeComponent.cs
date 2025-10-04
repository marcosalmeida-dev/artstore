// src/ArtStore.Domain/Entities/RecipeComponent.cs
using ArtStore.Domain.Common.Entities;
using ArtStore.Domain.Entities.Enums;

namespace ArtStore.Domain.Entities;

/// <summary>
/// Defines the Bill of Materials (BOM) or recipe for a finished product.
/// Specifies the raw material components consumed per ONE unit of the finished product.
/// Unique per (TenantId, ProductId, ComponentProductId).
/// </summary>
public class RecipeComponent : BaseTenantEntity<long>
{
    /// <summary>
    /// The finished product that requires this component.
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// The raw material or component product consumed.
    /// </summary>
    public int ComponentProductId { get; set; }

    /// <summary>
    /// Quantity of the component required per ONE unit of the finished product (precision 18,3).
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit of measure for the component quantity.
    /// </summary>
    public UnitOfMeasure Unit { get; set; }

    // Navigation properties
    /// <summary>
    /// The finished product entity.
    /// </summary>
    public virtual Product Product { get; set; } = null!;

    /// <summary>
    /// The component product entity.
    /// </summary>
    public virtual Product ComponentProduct { get; set; } = null!;
}