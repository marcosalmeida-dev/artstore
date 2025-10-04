using System.ComponentModel.DataAnnotations;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Inventory.Commands;

public class AddEditRecipeComponentCommand : ICommand<Result<long>>
{
    public long Id { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Required]
    public int ComponentProductId { get; set; }

    [Required]
    [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    [Required]
    public UnitOfMeasure Unit { get; set; }
}