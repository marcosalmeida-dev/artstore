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
    public string ProductCode { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public int? TenantId { get; set; }
    public string? TenantName { get; set; }
    public List<ProductImageDto> ImageDtos { get; set; } = new();
    public DateTime? Created { get; set; }
    public DateTime? LastModified { get; set; }

    public int Quantity { get; set; }
    public string PrimaryImageUrl
    {
        get
        {
            return ImageDtos?.FirstOrDefault(img => img.IsPrimary)?.Url
                ?? ImageDtos?.OrderBy(img => img.SortOrder).FirstOrDefault()?.Url
                ?? string.Empty;
        }
    }
}

public class ProductImageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public long Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string MimeType { get; set; } = "image/jpeg";
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}