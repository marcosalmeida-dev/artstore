using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Shared.DTOs.Inventory.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Commands.Delete;

public class DeleteInventoryItemCommandHandler : ICommandHandler<DeleteInventoryItemCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;

    public DeleteInventoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<long>> Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var item = await _context.InventoryItems
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
            if (item == null)
            {
                return Result<long>.Failure("Inventory item not found.");
            }

            // Check if there are stock movements
            var hasMovements = await _context.StockMovements
                .AnyAsync(m => m.InventoryItemId == request.Id, cancellationToken);
            if (hasMovements)
            {
                return Result<long>.Failure("Cannot delete an inventory item that has stock movements.");
            }

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<long>.Success(request.Id);
        }
        catch (Exception ex)
        {
            return Result<long>.Failure($"Error deleting inventory item: {ex.Message}");
        }
    }
}