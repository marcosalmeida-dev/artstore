using ArtStore.Application.Common.Interfaces;
using ArtStore.Domain.Extensions;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Products.Queries.GetAllForManagement;

public class GetAllProductsForManagementQuery : IQuery<IEnumerable<ProductDto>>
{
    public string Culture { get; set; } = "pt-BR";
}

public class GetAllProductsForManagementQueryHandler : IQueryHandler<GetAllProductsForManagementQuery, IEnumerable<ProductDto?>>
{
    private readonly IApplicationDbContext _context;

    public GetAllProductsForManagementQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto?>> Handle(GetAllProductsForManagementQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
                            .Include(p => p.Category)
                            .Where(p => true) // Include all products (active and inactive) for management
                            .ToListAsync(cancellationToken);

        return data.Select(x => new ProductDto
        {
            Id = x.Id,
            Name = x.GetLocalizedName(request.Culture) ?? x.Name ?? string.Empty,
            ProductCode = x.ProductCode,
            Description = x.GetLocalizedDescription(request.Culture) ?? x.Description,
            Brand = x.Brand,
            CategoryId = x.CategoryId,
            CategoryName = x.Category?.GetLocalizedName(request.Culture) ?? x.Category?.Name,
            Price = x.Price,
            IsActive = x.IsActive,
            Created = x.Created
        }).ToList();
    }
}