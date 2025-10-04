using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Domain.Entities;
using ArtStore.Shared.DTOs.Inventory.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Commands.AddEdit;

public class AddEditInventoryLocationCommandHandler : ICommandHandler<AddEditInventoryLocationCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddEditInventoryLocationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddEditInventoryLocationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if code is unique (if provided)
            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var query = _context.InventoryLocations.Where(l => l.Code!.ToLower() == request.Code.ToLower());
                if (request.Id != 0)
                {
                    query = query.Where(l => l.Id != request.Id);
                }
                var codeExists = await query.AnyAsync(cancellationToken);
                if (codeExists)
                {
                    return Result<int>.Failure("Location code already exists.");
                }
            }

            if (request.Id == 0)
            {
                // Create new location
                var location = new InventoryLocation
                {
                    Name = request.Name,
                    Code = request.Code?.ToUpper(),
                    IsDefault = request.IsDefault,
                    IsActive = request.IsActive
                };

                _context.InventoryLocations.Add(location);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(location.Id);
            }
            else
            {
                // Update existing location
                var existingLocation = await _context.InventoryLocations
                    .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
                if (existingLocation == null)
                {
                    return Result<int>.Failure("Location not found.");
                }

                existingLocation.Name = request.Name;
                existingLocation.Code = request.Code?.ToUpper();
                existingLocation.IsDefault = request.IsDefault;
                existingLocation.IsActive = request.IsActive;

                _context.InventoryLocations.Update(existingLocation);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(existingLocation.Id);
            }
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error saving location: {ex.Message}");
        }
    }
}