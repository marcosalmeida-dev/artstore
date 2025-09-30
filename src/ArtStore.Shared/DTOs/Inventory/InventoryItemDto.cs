namespace ArtStore.Shared.DTOs.Inventory;

public class InventoryItemDto
{
    public long Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductCode { get; set; } = string.Empty;
    public int InventoryLocationId { get; set; }
    public string LocationName { get; set; } = string.Empty;
    public decimal OnHand { get; set; }
    public decimal SafetyStock { get; set; }
    public decimal ReorderPoint { get; set; }
    public decimal Available { get; set; }
    public bool IsLowStock => OnHand <= ReorderPoint;
    public bool IsBelowSafety => OnHand <= SafetyStock;
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}