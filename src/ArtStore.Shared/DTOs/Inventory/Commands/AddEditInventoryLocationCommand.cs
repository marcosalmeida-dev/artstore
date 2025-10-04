using System.ComponentModel.DataAnnotations;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Inventory.Commands;

public class AddEditInventoryLocationCommand : ICommand<Result<int>>
{
    public int Id { get; set; }

    [Required]
    [StringLength(128, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(32)]
    public string? Code { get; set; }

    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
}