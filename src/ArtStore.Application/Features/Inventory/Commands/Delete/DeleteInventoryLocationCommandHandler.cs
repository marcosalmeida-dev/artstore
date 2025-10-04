using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Shared.DTOs.Inventory.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Commands.Delete;

public class DeleteInventoryLocationCommandHandler : ICommandHandler<DeleteInventoryLocationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeleteInventoryLocationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteInventoryLocationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var location = await _context.InventoryLocations
                .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
            if (location == null)
            {
                return Result<int>.Failure("Location not found.");
            }

            // Check if location has inventory items
            var hasItems = await _context.InventoryItems
                .AnyAsync(i => i.InventoryLocationId == request.Id, cancellationToken);
            if (hasItems)
            {
                return Result<int>.Failure("Cannot delete a location that has inventory items.");
            }

            _context.InventoryLocations.Remove(location);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(request.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error deleting location: {ex.Message}");
        }
    }
}