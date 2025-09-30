// src/ArtStore.Domain/Entities/Enums/UnitOfMeasure.cs
namespace ArtStore.Domain.Entities.Enums;

/// <summary>
/// Units of measure for inventory items and recipe components.
/// </summary>
public enum UnitOfMeasure
{
    /// <summary>
    /// Individual piece or item (default, no conversion)
    /// </summary>
    Piece = 1,

    /// <summary>
    /// Gram (weight)
    /// </summary>
    Gram = 2,

    /// <summary>
    /// Kilogram (weight) - 1 kg = 1000 g
    /// </summary>
    Kilogram = 3,

    /// <summary>
    /// Milliliter (volume)
    /// </summary>
    Milliliter = 4,

    /// <summary>
    /// Liter (volume) - 1 l = 1000 ml
    /// </summary>
    Liter = 5
}