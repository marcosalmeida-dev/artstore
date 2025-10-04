using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Inventory.Commands;

public class DeleteInventoryItemCommand : ICommand<Result<long>>
{
    public long Id { get; set; }
}