using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Category.Commands;

public class AddEditCategoryCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int? ParentCategoryId { get; set; }
}