using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Shared.DTOs.Inventory.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Commands.Delete;

public class DeleteRecipeComponentCommandHandler : ICommandHandler<DeleteRecipeComponentCommand, Result<long>>
{
    private readonly IApplicationDbContext _context;

    public DeleteRecipeComponentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<long>> Handle(DeleteRecipeComponentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var component = await _context.RecipeComponents
                .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
            if (component == null)
            {
                return Result<long>.Failure("Recipe component not found.");
            }

            _context.RecipeComponents.Remove(component);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<long>.Success(request.Id);
        }
        catch (Exception ex)
        {
            return Result<long>.Failure($"Error deleting recipe component: {ex.Message}");
        }
    }
}