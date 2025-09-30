// src/ArtStore.Domain/Entities/Enums/InventoryEnums.cs
namespace ArtStore.Domain.Entities.Enums;

/// <summary>
/// Types of stock movements in the inventory system.
/// </summary>
public enum StockMovementType
{
    /// <summary>
    /// Stock received into inventory (e.g., purchase, production)
    /// </summary>
    Receipt = 1,

    /// <summary>
    /// Stock sold to customer
    /// </summary>
    Sale = 2,

    /// <summary>
    /// Stock returned to inventory
    /// </summary>
    Return = 3,

    /// <summary>
    /// Manual stock adjustment (correction, damage, etc.)
    /// </summary>
    Adjustment = 4,

    /// <summary>
    /// Stock transferred into this location
    /// </summary>
    TransferIn = 5,

    /// <summary>
    /// Stock transferred out of this location
    /// </summary>
    TransferOut = 6,

    /// <summary>
    /// Reservation created (soft hold)
    /// </summary>
    ReservationCreate = 7,

    /// <summary>
    /// Reservation committed (hard deduction)
    /// </summary>
    ReservationCommit = 8,

    /// <summary>
    /// Reservation released (soft hold removed)
    /// </summary>
    ReservationRelease = 9
}

/// <summary>
/// Status of an inventory reservation.
/// </summary>
public enum ReservationStatus
{
    /// <summary>
    /// Reservation is active and holding stock
    /// </summary>
    Active = 1,

    /// <summary>
    /// Reservation has been committed and stock deducted
    /// </summary>
    Committed = 2,

    /// <summary>
    /// Reservation has been released
    /// </summary>
    Released = 3,

    /// <summary>
    /// Reservation has expired due to timeout
    /// </summary>
    Expired = 4
}