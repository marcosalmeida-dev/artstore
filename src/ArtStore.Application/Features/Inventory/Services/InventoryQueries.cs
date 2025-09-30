// src/ArtStore.Application/Features/Inventory/Services/InventoryQueries.cs
using ArtStore.Domain.Entities;
using ArtStore.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Services;

/// <summary>
/// Static query helpers for inventory operations.
/// </summary>
public static class InventoryQueries
{
    /// <summary>
    /// Gets the inventory snapshot for a product at a location.
    /// </summary>
    /// <param name="db">The database context.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="productId">The product ID.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A tuple containing:
    /// - onHand: Current quantity on hand
    /// - available: Available quantity (onHand - sum of active reservations)
    /// </returns>
    public static async Task<(decimal onHand, decimal available)> GetSnapshotAsync(
        DbContext db,
        int tenantId,
        int productId,
        int locationId,
        CancellationToken cancellationToken = default)
    {
        // Get inventory item
        var item = await db.Set<InventoryItem>()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId &&
                     x.ProductId == productId &&
                     x.InventoryLocationId == locationId,
                cancellationToken);

        var onHand = item?.OnHand ?? 0m;

        // Sum active reservations
        var reserved = await db.Set<InventoryReservation>()
            .Where(x => x.TenantId == tenantId &&
                       x.ProductId == productId &&
                       x.InventoryLocationId == locationId &&
                       x.Status == ReservationStatus.Active)
            .SumAsync(x => x.Quantity, cancellationToken);

        var available = onHand - reserved;

        return (onHand, available);
    }
}