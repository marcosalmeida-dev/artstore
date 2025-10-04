using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Inventory.Commands;

public class DeleteInventoryLocationCommand : ICommand<Result<int>>
{
    public int Id { get; set; }
}