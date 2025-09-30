// src/ArtStore.Application/Features/Inventory/Services/InventoryService.cs
using ArtStore.Domain.Entities;
using ArtStore.Domain.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Services;

/// <summary>
/// Service for inventory management operations.
/// </summary>
public partial class InventoryService
{
    private readonly DbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryService"/> class.
    /// </summary>
    /// <param name="db">The database context.</param>
    public InventoryService(DbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Rounds a decimal value to 2 decimal places using MidpointRounding.AwayFromZero.
    /// </summary>
    private static decimal Round2(decimal value) =>
        Math.Round(value, 2, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Gets or creates an inventory item for a product at a location.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The inventory item.</returns>
    public async Task<InventoryItem> GetOrCreateItemAsync(
        int productId,
        int locationId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var item = await _db.Set<InventoryItem>()
            .FirstOrDefaultAsync(
                x => x.TenantId == tenantId &&
                     x.ProductId == productId &&
                     x.InventoryLocationId == locationId,
                cancellationToken);

        if (item == null)
        {
            item = new InventoryItem
            {
                ProductId = productId,
                InventoryLocationId = locationId,
                TenantId = tenantId,
                OnHand = 0,
                SafetyStock = 0,
                ReorderPoint = 0
            };
            _db.Set<InventoryItem>().Add(item);
            await _db.SaveChangesAsync(cancellationToken);
        }

        return item;
    }

    /// <summary>
    /// Receives stock into inventory.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="quantity">The quantity to receive (must be positive).</param>
    /// <param name="reference">Optional external reference (e.g., PO number).</param>
    /// <param name="notes">Optional notes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task ReceiveAsync(
        int productId,
        int locationId,
        int tenantId,
        decimal quantity,
        string? reference = null,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        var item = await GetOrCreateItemAsync(productId, locationId, tenantId, cancellationToken);

        item.OnHand = Round2(item.OnHand + quantity);

        var movement = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = item.Id,
            ProductId = productId,
            InventoryLocationId = locationId,
            Quantity = quantity,
            Type = StockMovementType.Receipt,
            Reference = reference,
            Notes = notes,
            OccurredAt = DateTimeOffset.UtcNow
        };

        _db.Set<StockMovement>().Add(movement);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Reserves inventory for an order detail.
    /// </summary>
    /// <param name="orderId">The order ID.</param>
    /// <param name="orderDetailId">The order detail ID.</param>
    /// <param name="productId">The product ID.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="quantity">The quantity to reserve.</param>
    /// <param name="ttl">Optional time-to-live (expiration duration).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created reservation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when insufficient available inventory.</exception>
    public async Task<InventoryReservation> ReserveAsync(
        long orderId,
        long orderDetailId,
        int productId,
        int locationId,
        int tenantId,
        decimal quantity,
        TimeSpan? ttl = null,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        // Check available inventory
        var (onHand, available) = await InventoryQueries.GetSnapshotAsync(
            _db, tenantId, productId, locationId, cancellationToken);

        if (available < quantity)
        {
            throw new InvalidOperationException(
                $"Insufficient inventory. Available: {available}, Requested: {quantity}");
        }

        var item = await GetOrCreateItemAsync(productId, locationId, tenantId, cancellationToken);

        var reservation = new InventoryReservation
        {
            TenantId = tenantId,
            ProductId = productId,
            InventoryLocationId = locationId,
            OrderId = orderId,
            OrderDetailId = orderDetailId,
            Quantity = quantity,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = ttl.HasValue ? DateTimeOffset.UtcNow.Add(ttl.Value) : null,
            Status = ReservationStatus.Active
        };

        _db.Set<InventoryReservation>().Add(reservation);

        var movement = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = item.Id,
            ProductId = productId,
            InventoryLocationId = locationId,
            Quantity = 0, // No immediate stock change
            Type = StockMovementType.ReservationCreate,
            OrderId = orderId,
            OrderDetailId = orderDetailId,
            Notes = $"Reserved {quantity} units",
            OccurredAt = DateTimeOffset.UtcNow
        };

        _db.Set<StockMovement>().Add(movement);
        await _db.SaveChangesAsync(cancellationToken);

        return reservation;
    }

    /// <summary>
    /// Commits a reservation (converts soft hold to hard deduction).
    /// </summary>
    /// <param name="reservationId">The reservation ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when reservation not found or not active.</exception>
    public async Task CommitReservationAsync(
        long reservationId,
        int tenantId,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _db.Set<InventoryReservation>()
            .FirstOrDefaultAsync(
                x => x.Id == reservationId && x.TenantId == tenantId,
                cancellationToken);

        if (reservation == null)
            throw new InvalidOperationException($"Reservation {reservationId} not found.");

        if (reservation.Status != ReservationStatus.Active)
            throw new InvalidOperationException(
                $"Reservation {reservationId} is not active (status: {reservation.Status}).");

        var item = await GetOrCreateItemAsync(
            reservation.ProductId,
            reservation.InventoryLocationId,
            tenantId,
            cancellationToken);

        // Deduct from on-hand
        item.OnHand = Round2(item.OnHand - reservation.Quantity);

        if (item.OnHand < 0)
            throw new InvalidOperationException(
                $"Cannot commit reservation: would result in negative inventory (OnHand: {item.OnHand}).");

        // Mark reservation as committed
        reservation.Status = ReservationStatus.Committed;

        // Add movements
        var commitMovement = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = item.Id,
            ProductId = reservation.ProductId,
            InventoryLocationId = reservation.InventoryLocationId,
            Quantity = 0,
            Type = StockMovementType.ReservationCommit,
            OrderId = reservation.OrderId,
            OrderDetailId = reservation.OrderDetailId,
            Notes = $"Committed reservation {reservationId}",
            OccurredAt = DateTimeOffset.UtcNow
        };

        var saleMovement = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = item.Id,
            ProductId = reservation.ProductId,
            InventoryLocationId = reservation.InventoryLocationId,
            Quantity = -reservation.Quantity,
            Type = StockMovementType.Sale,
            OrderId = reservation.OrderId,
            OrderDetailId = reservation.OrderDetailId,
            Notes = $"Sale for order {reservation.OrderId}",
            OccurredAt = DateTimeOffset.UtcNow
        };

        _db.Set<StockMovement>().Add(commitMovement);
        _db.Set<StockMovement>().Add(saleMovement);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Releases a reservation (removes soft hold).
    /// </summary>
    /// <param name="reservationId">The reservation ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="reason">Reason for release.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when reservation not found or not active.</exception>
    public async Task ReleaseReservationAsync(
        long reservationId,
        int tenantId,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var reservation = await _db.Set<InventoryReservation>()
            .FirstOrDefaultAsync(
                x => x.Id == reservationId && x.TenantId == tenantId,
                cancellationToken);

        if (reservation == null)
            throw new InvalidOperationException($"Reservation {reservationId} not found.");

        if (reservation.Status != ReservationStatus.Active)
            throw new InvalidOperationException(
                $"Reservation {reservationId} is not active (status: {reservation.Status}).");

        var item = await GetOrCreateItemAsync(
            reservation.ProductId,
            reservation.InventoryLocationId,
            tenantId,
            cancellationToken);

        // Mark reservation as released
        reservation.Status = ReservationStatus.Released;

        var movement = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = item.Id,
            ProductId = reservation.ProductId,
            InventoryLocationId = reservation.InventoryLocationId,
            Quantity = 0,
            Type = StockMovementType.ReservationRelease,
            OrderId = reservation.OrderId,
            OrderDetailId = reservation.OrderDetailId,
            Notes = reason ?? $"Released reservation {reservationId}",
            OccurredAt = DateTimeOffset.UtcNow
        };

        _db.Set<StockMovement>().Add(movement);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Adjusts inventory by a delta amount.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="locationId">The location ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="delta">The adjustment amount (positive or negative).</param>
    /// <param name="reason">Reason for adjustment.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when adjustment would result in negative inventory.</exception>
    public async Task AdjustAsync(
        int productId,
        int locationId,
        int tenantId,
        decimal delta,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        var item = await GetOrCreateItemAsync(productId, locationId, tenantId, cancellationToken);

        var newOnHand = Round2(item.OnHand + delta);

        if (newOnHand < 0)
            throw new InvalidOperationException(
                $"Adjustment would result in negative inventory (current: {item.OnHand}, delta: {delta}).");

        item.OnHand = newOnHand;

        var movement = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = item.Id,
            ProductId = productId,
            InventoryLocationId = locationId,
            Quantity = delta,
            Type = StockMovementType.Adjustment,
            Notes = reason ?? "Manual adjustment",
            OccurredAt = DateTimeOffset.UtcNow
        };

        _db.Set<StockMovement>().Add(movement);
        await _db.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Transfers inventory between locations.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="fromLocationId">The source location ID.</param>
    /// <param name="toLocationId">The destination location ID.</param>
    /// <param name="tenantId">The tenant ID.</param>
    /// <param name="quantity">The quantity to transfer (must be positive).</param>
    /// <param name="notes">Optional notes.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="InvalidOperationException">Thrown when insufficient inventory at source location.</exception>
    public async Task TransferAsync(
        int productId,
        int fromLocationId,
        int toLocationId,
        int tenantId,
        decimal quantity,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive.", nameof(quantity));

        if (fromLocationId == toLocationId)
            throw new ArgumentException("Source and destination locations must be different.");

        var fromItem = await GetOrCreateItemAsync(productId, fromLocationId, tenantId, cancellationToken);
        var toItem = await GetOrCreateItemAsync(productId, toLocationId, tenantId, cancellationToken);

        if (fromItem.OnHand < quantity)
            throw new InvalidOperationException(
                $"Insufficient inventory at source location (available: {fromItem.OnHand}, requested: {quantity}).");

        fromItem.OnHand = Round2(fromItem.OnHand - quantity);
        toItem.OnHand = Round2(toItem.OnHand + quantity);

        var transferOut = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = fromItem.Id,
            ProductId = productId,
            InventoryLocationId = fromLocationId,
            Quantity = -quantity,
            Type = StockMovementType.TransferOut,
            Notes = notes ?? $"Transfer to location {toLocationId}",
            OccurredAt = DateTimeOffset.UtcNow
        };

        var transferIn = new StockMovement
        {
            TenantId = tenantId,
            InventoryItemId = toItem.Id,
            ProductId = productId,
            InventoryLocationId = toLocationId,
            Quantity = quantity,
            Type = StockMovementType.TransferIn,
            Notes = notes ?? $"Transfer from location {fromLocationId}",
            OccurredAt = DateTimeOffset.UtcNow
        };

        _db.Set<StockMovement>().Add(transferOut);
        _db.Set<StockMovement>().Add(transferIn);
        await _db.SaveChangesAsync(cancellationToken);
    }
}