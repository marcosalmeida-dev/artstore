using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Inventory;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Inventory.Queries.GetAll;

public class GetAllRecipeComponentsQuery : IQuery<IEnumerable<RecipeComponentDto>>
{
}

public class GetRecipeComponentQuery : IQuery<RecipeComponentDto>
{
    public required long Id { get; set; }
}

public class GetRecipeComponentsByProductQuery : IQuery<IEnumerable<RecipeComponentDto>>
{
    public required int ProductId { get; set; }
}

public class GetAllRecipeComponentsQueryHandler :
    IQueryHandler<GetAllRecipeComponentsQuery, IEnumerable<RecipeComponentDto?>>,
    IQueryHandler<GetRecipeComponentQuery, RecipeComponentDto?>,
    IQueryHandler<GetRecipeComponentsByProductQuery, IEnumerable<RecipeComponentDto?>>
{
    private readonly IApplicationDbContext _context;

    public GetAllRecipeComponentsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RecipeComponentDto?>> Handle(GetAllRecipeComponentsQuery request, CancellationToken cancellationToken)
    {
        var components = await _context.RecipeComponents
            .Include(r => r.Product)
            .Include(r => r.ComponentProduct)
            .OrderBy(r => r.Product.Name)
            .ThenBy(r => r.ComponentProduct.Name)
            .ToListAsync(cancellationToken);

        return components.Select(r => new RecipeComponentDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            ProductName = r.Product.Name ?? string.Empty,
            ComponentProductId = r.ComponentProductId,
            ComponentProductName = r.ComponentProduct.Name ?? string.Empty,
            Quantity = r.Quantity,
            Unit = (Shared.DTOs.Inventory.UnitOfMeasure)r.Unit,
            Created = r.Created,
            LastModified = r.LastModified
        }).ToList();
    }

    public async Task<RecipeComponentDto?> Handle(GetRecipeComponentQuery request, CancellationToken cancellationToken)
    {
        var component = await _context.RecipeComponents
            .Include(r => r.Product)
            .Include(r => r.ComponentProduct)
            .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (component == null) return null;

        return new RecipeComponentDto
        {
            Id = component.Id,
            ProductId = component.ProductId,
            ProductName = component.Product.Name ?? string.Empty,
            ComponentProductId = component.ComponentProductId,
            ComponentProductName = component.ComponentProduct.Name ?? string.Empty,
            Quantity = component.Quantity,
            Unit = (Shared.DTOs.Inventory.UnitOfMeasure)component.Unit,
            Created = component.Created,
            LastModified = component.LastModified
        };
    }

    public async Task<IEnumerable<RecipeComponentDto?>> Handle(GetRecipeComponentsByProductQuery request, CancellationToken cancellationToken)
    {
        var components = await _context.RecipeComponents
            .Include(r => r.Product)
            .Include(r => r.ComponentProduct)
            .Where(r => r.ProductId == request.ProductId)
            .OrderBy(r => r.ComponentProduct.Name)
            .ToListAsync(cancellationToken);

        return components.Select(r => new RecipeComponentDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            ProductName = r.Product.Name ?? string.Empty,
            ComponentProductId = r.ComponentProductId,
            ComponentProductName = r.ComponentProduct.Name ?? string.Empty,
            Quantity = r.Quantity,
            Unit = (Shared.DTOs.Inventory.UnitOfMeasure)r.Unit,
            Created = r.Created,
            LastModified = r.LastModified
        }).ToList();
    }
}