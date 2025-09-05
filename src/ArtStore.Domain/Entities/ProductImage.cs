namespace ArtStore.Domain.Entities;

public class ProductImage : BaseEntity<int>
{
    public int ProductId { get; set; }
    public required string Name { get; set; }
    public required string FileName { get; set; }
    public required string Url { get; set; }
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public long Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string MimeType { get; set; } = "image/jpeg";
    public string? Hash { get; set; } // For duplicate detection
    public bool IsPrimary { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    // Navigation properties
    public virtual Product Product { get; set; } = null!;
}