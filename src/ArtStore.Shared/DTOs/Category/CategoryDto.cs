namespace ArtStore.Shared.DTOs.Category;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int? TenantId { get; set; }
    public string? TenantName { get; set; }
    public List<CategoryDto> SubCategories { get; set; } = new();
    public int ProductCount { get; set; }
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}