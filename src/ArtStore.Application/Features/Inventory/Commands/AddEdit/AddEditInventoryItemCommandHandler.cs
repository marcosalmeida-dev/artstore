using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Domain.Entities;
using ArtStore.Shared.DTOs.Inventory.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Commands.AddEdit;

public class AddEditInventoryItemCommandHandler : ICommandHandler<AddEditInventoryItemCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;

    public AddEditInventoryItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<long>> Handle(AddEditInventoryItemCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if product exists
            var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
            if (!productExists)
            {
                return Result<long>.Failure("Product not found.");
            }

            // Check if location exists
            var locationExists = await _context.InventoryLocations.AnyAsync(l => l.Id == request.InventoryLocationId, cancellationToken);
            if (!locationExists)
            {
                return Result<long>.Failure("Location not found.");
            }

            // Check for duplicate (same product + location)
            var duplicateQuery = _context.InventoryItems
                .Where(i => i.ProductId == request.ProductId && i.InventoryLocationId == request.InventoryLocationId);
            if (request.Id != 0)
            {
                duplicateQuery = duplicateQuery.Where(i => i.Id != request.Id);
            }
            var duplicateExists = await duplicateQuery.AnyAsync(cancellationToken);
            if (duplicateExists)
            {
                return Result<long>.Failure("Inventory item already exists for this product and location combination.");
            }

            if (request.Id == 0)
            {
                // Create new inventory item
                var item = new InventoryItem
                {
                    ProductId = request.ProductId,
                    InventoryLocationId = request.InventoryLocationId,
                    OnHand = request.OnHand,
                    SafetyStock = request.SafetyStock,
                    ReorderPoint = request.ReorderPoint
                };

                _context.InventoryItems.Add(item);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<long>.Success(item.Id);
            }
            else
            {
                // Update existing item
                var existingItem = await _context.InventoryItems
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);
                if (existingItem == null)
                {
                    return Result<long>.Failure("Inventory item not found.");
                }

                existingItem.ProductId = request.ProductId;
                existingItem.InventoryLocationId = request.InventoryLocationId;
                existingItem.OnHand = request.OnHand;
                existingItem.SafetyStock = request.SafetyStock;
                existingItem.ReorderPoint = request.ReorderPoint;

                _context.InventoryItems.Update(existingItem);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<long>.Success(existingItem.Id);
            }
        }
        catch (Exception ex)
        {
            return Result<long>.Failure($"Error saving inventory item: {ex.Message}");
        }
    }
}