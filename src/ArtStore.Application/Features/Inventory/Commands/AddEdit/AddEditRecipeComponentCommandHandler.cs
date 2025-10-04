using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Domain.Entities;
using ArtStore.Shared.DTOs.Inventory.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Commands.AddEdit;

public class AddEditRecipeComponentCommandHandler : ICommandHandler<AddEditRecipeComponentCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;

    public AddEditRecipeComponentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<long>> Handle(AddEditRecipeComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if product exists
            var productExists = await _context.Products.AnyAsync(p => p.Id == request.ProductId, cancellationToken);
            if (!productExists)
            {
                return Result<long>.Failure("Finished product not found.");
            }

            // Check if component product exists
            var componentExists = await _context.Products.AnyAsync(p => p.Id == request.ComponentProductId, cancellationToken);
            if (!componentExists)
            {
                return Result<long>.Failure("Component product not found.");
            }

            // Prevent circular references
            if (request.ProductId == request.ComponentProductId)
            {
                return Result<long>.Failure("A product cannot be a component of itself.");
            }

            // Check for duplicate (same product + component)
            var duplicateQuery = _context.RecipeComponents
                .Where(r => r.ProductId == request.ProductId && r.ComponentProductId == request.ComponentProductId);
            if (request.Id != 0)
            {
                duplicateQuery = duplicateQuery.Where(r => r.Id != request.Id);
            }
            var duplicateExists = await duplicateQuery.AnyAsync(cancellationToken);
            if (duplicateExists)
            {
                return Result<long>.Failure("Recipe component already exists for this product and component combination.");
            }

            if (request.Id == 0)
            {
                // Create new recipe component
                var component = new RecipeComponent
                {
                    ProductId = request.ProductId,
                    ComponentProductId = request.ComponentProductId,
                    Quantity = request.Quantity,
                    Unit = (Domain.Entities.Enums.UnitOfMeasure)request.Unit
                };

                _context.RecipeComponents.Add(component);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<long>.Success(component.Id);
            }
            else
            {
                // Update existing component
                var existingComponent = await _context.RecipeComponents
                    .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
                if (existingComponent == null)
                {
                    return Result<long>.Failure("Recipe component not found.");
                }

                existingComponent.ProductId = request.ProductId;
                existingComponent.ComponentProductId = request.ComponentProductId;
                existingComponent.Quantity = request.Quantity;
                existingComponent.Unit = (Domain.Entities.Enums.UnitOfMeasure)request.Unit;

                _context.RecipeComponents.Update(existingComponent);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<long>.Success(existingComponent.Id);
            }
        }
        catch (Exception ex)
        {
            return Result<long>.Failure($"Error saving recipe component: {ex.Message}");
        }
    }
}