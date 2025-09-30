namespace ArtStore.Shared.DTOs.Inventory;

public class InventoryLocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}