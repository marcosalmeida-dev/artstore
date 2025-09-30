// src/ArtStore.Application/Features/Inventory/Services/UnitConversion.cs
using ArtStore.Domain.Entities.Enums;

namespace ArtStore.Application.Features.Inventory.Services;

/// <summary>
/// Static helper for converting between units of measure.
/// </summary>
public static class UnitConversion
{
    /// <summary>
    /// Converts a quantity from one unit of measure to another.
    /// </summary>
    /// <param name="quantity">The quantity to convert.</param>
    /// <param name="from">The source unit.</param>
    /// <param name="to">The target unit.</param>
    /// <returns>The converted quantity.</returns>
    /// <exception cref="NotSupportedException">Thrown when conversion between the specified units is not supported.</exception>
    public static decimal Convert(decimal quantity, UnitOfMeasure from, UnitOfMeasure to)
    {
        // Same unit, no conversion needed
        if (from == to)
            return quantity;

        // Gram <-> Kilogram conversions
        if (from == UnitOfMeasure.Gram && to == UnitOfMeasure.Kilogram)
            return quantity / 1000m;
        if (from == UnitOfMeasure.Kilogram && to == UnitOfMeasure.Gram)
            return quantity * 1000m;

        // Milliliter <-> Liter conversions
        if (from == UnitOfMeasure.Milliliter && to == UnitOfMeasure.Liter)
            return quantity / 1000m;
        if (from == UnitOfMeasure.Liter && to == UnitOfMeasure.Milliliter)
            return quantity * 1000m;

        // Piece is always 1:1 (identity)
        if (from == UnitOfMeasure.Piece && to == UnitOfMeasure.Piece)
            return quantity;

        // Unsupported conversion
        throw new NotSupportedException(
            $"Conversion from {from} to {to} is not supported. " +
            "Supported conversions: Gram<->Kilogram, Milliliter<->Liter, Piece (identity only).");
    }
}