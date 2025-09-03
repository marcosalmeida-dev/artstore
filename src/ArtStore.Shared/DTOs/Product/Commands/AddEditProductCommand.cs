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
    public List<ProductImageDto> Pictures { get; set; } = new();
}