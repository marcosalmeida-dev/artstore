namespace ArtStore.Shared.DTOs.Product;

[Description("Products")]
public class ProductDto
{
    [Description("Id")] public int Id { get; set; }

    [Description("Product Name")] public string? Name { get; set; }

    [Description("Description")] public string? Description { get; set; }

    [Description("Unit")] public string? Unit { get; set; }

    [Description("Brand Name")] public string? Brand { get; set; }

    [Description("Price")] public decimal Price { get; set; }

    [Description("ImageUrl")] public string ImageUrl { get; set; }
}