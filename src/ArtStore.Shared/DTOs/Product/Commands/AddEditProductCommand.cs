using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Product.Commands;

public class AddEditProductCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public List<ProductImageDto> ImagesDto { get; set; } = new();
    public List<UploadProductImageDto> NewImages { get; set; } = new();
    public List<int> ImagesToDelete { get; set; } = new();
    public List<UpdateProductImageDto> ImagesToUpdate { get; set; } = new();
}

public class UploadProductImageDto
{
    public string Name { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string ContentType { get; set; } = "image/jpeg";
    public string? AltText { get; set; }
    public string? Caption { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

public class UpdateProductImageDto
{
    public int Id { get; set; }
    public bool IsPrimary { get; set; }
}