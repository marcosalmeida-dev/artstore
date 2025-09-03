namespace ArtStore.Shared.DTOs.Product;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? TenantId { get; set; }
    public string? TenantName { get; set; }
    public List<ProductImageDto> Pictures { get; set; } = new();
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }
}

public class ProductImageDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Size { get; set; }
    public string Url { get; set; } = string.Empty;
}