namespace ArtStore.Shared.DTOs.Inventory;

public class RecipeComponentDto
{
    public long Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int ComponentProductId { get; set; }
    public string ComponentProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public UnitOfMeasure Unit { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}

public enum UnitOfMeasure
{
    Piece = 1,
    Gram = 2,
    Kilogram = 3,
    Milliliter = 4,
    Liter = 5
}