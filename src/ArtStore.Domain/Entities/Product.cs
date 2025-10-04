using ArtStore.Domain.Entities.Translations;

namespace ArtStore.Domain.Entities;

public class Product : BaseTenantEntity<int>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
    public int CategoryId { get; set; }

    // JSON column for translations
    public ProductTranslationsJson? Translations { get; set; }

    // Navigation properties
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}