// src/ArtStore.Application/Features/Inventory/Services/InventoryService.BomExtensions.cs
using ArtStore.Domain.Entities;
using ArtStore.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Services;

/// <summary>
/// BOM (Bill of Materials) extensions for InventoryService.
/// </summary>
public partial class InventoryService
{
    /// <summary>
    /// Gets the recipe (BOM) for a finished product.
    /// </summary>
    /// <param name="productId">The finished product ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of recipe components.</returns>
    public async Task<List<RecipeComponent>> GetRecipeAsync(
        int productId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        return await _db.Set<RecipeComponent>()
            .Where(x => x.TenantId == tenantId && x.ProductId == productId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Expands a finished product into its component requirements.
    /// If the product has no recipe, it's treated as a direct stock item.
    /// </summary>
    /// <param name="productId">The finished product ID.</param>
    /// <param name="productQty">The quantity of finished product needed.</param>
    /// <param name="desiredUnitForDirect">Unit to use if product has no recipe (direct item).</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of (ComponentProductId, Quantity, Unit) tuples.</returns>
    public async Task<List<(int ComponentProductId, decimal Quantity, UnitOfMeasure Unit)>> ExpandRequirementsAsync(
        int productId,
        decimal productQty,
        UnitOfMeasure desiredUnitForDirect,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var recipe = await GetRecipeAsync(productId, tenantId, cancellationToken);

        // If no recipe, treat as direct stock item (1:1)
        if (recipe.Count == 0)
        {
            return new List<(int, decimal, UnitOfMeasure)>
            {
                (productId, productQty, desiredUnitForDirect)
            };
        }

        // Expand recipe components
        var requirements = new List<(int, decimal, UnitOfMeasure)>();
        foreach (var component in recipe)
        {
            var requiredQty = component.Quantity * productQty;
            requirements.Add((component.ComponentProductId, requiredQty, component.Unit));
        }

        return requirements;
    }

    /// <summary>
    /// Reserves inventory for an order detail, expanding BOM if applicable.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="orderDetailId">The order detail ID.</param>
    /// <param name="finishedProductId">The finished product ID from the order.</param>
    /// <param name="orderDetailQty">The quantity ordered.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="ttl">Optional time-to-live for reservations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of created reservations.</returns>
    public async Task<List<InventoryReservation>> ReserveForOrderDetailAsync(
        long orderId,
        long orderDetailId,
        int finishedProductId,
        decimal orderDetailQty,
        int locationId,
        int tenantId,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        // Expand requirements (BOM or direct)
        var requirements = await ExpandRequirementsAsync(
            finishedProductId,
            orderDetailQty,
            UnitOfMeasure.Piece, // Default unit for direct items
            tenantId,
            cancellationToken);

        var reservations = new List<InventoryReservation>();

        // Reserve each component
        foreach (var (componentProductId, quantity, unit) in requirements)
        {
            var reservation = await ReserveAsync(
                orderId,
                orderDetailId,
                componentProductId,
                locationId,
                tenantId,
                quantity,
                ttl,
                cancellationToken);

            reservations.Add(reservation);
        }

        return reservations;
    }

    /// <summary>
    /// Commits all active reservations for an order.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task CommitForOrderAsync(
        long orderId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var activeReservations = await _db.Set<InventoryReservation>()
            .Where(x => x.TenantId == tenantId &&
                       x.OrderId == orderId &&
                       x.Status == ReservationStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var reservation in activeReservations)
        {
            await CommitReservationAsync(reservation.Id, tenantId, cancellationToken);
        }
    }

    /// <summary>
    /// Releases all active reservations for an order.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="reason">Reason for release.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ReleaseForOrderAsync(
        long orderId,
        int tenantId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var activeReservations = await _db.Set<InventoryReservation>()
            .Where(x => x.TenantId == tenantId &&
                       x.OrderId == orderId &&
                       x.Status == ReservationStatus.Active)
            .ToListAsync(cancellationToken);

        foreach (var reservation in activeReservations)
        {
            await ReleaseReservationAsync(reservation.Id, tenantId, reason, cancellationToken);
        }
    }
}