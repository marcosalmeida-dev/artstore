using System.ComponentModel.DataAnnotations;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Inventory.Commands;

public class AddEditInventoryItemCommand : ICommand<Result<long>>
{
    public long Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int InventoryLocationId { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "OnHand must be 0 or greater")]
    public decimal OnHand { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "SafetyStock must be 0 or greater")]
    public decimal SafetyStock { get; set; }

    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "ReorderPoint must be 0 or greater")]
    public decimal ReorderPoint { get; set; }
}