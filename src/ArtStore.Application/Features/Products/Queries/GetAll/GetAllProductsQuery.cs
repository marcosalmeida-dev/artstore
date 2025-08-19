using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Products.Caching;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Products.Queries.GetAll;

public class GetAllProductsQuery : IQuery<IEnumerable<ProductDto>>
{

}

public class GetProductQuery : IQuery<ProductDto>
{
    public required int Id { get; set; }

    public string CacheKey => ProductCacheKey.GetProductByIdCacheKey(Id);
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class GetAllProductsQueryHandler :
    IQueryHandler<GetAllProductsQuery, IEnumerable<ProductDto?>>,
    IQueryHandler<GetProductQuery, ProductDto?>

{
    private readonly IApplicationDbContext _context;

    public GetAllProductsQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
            .ToListAsync(cancellationToken);

        return data.Select(x => new ProductDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            ImageUrl = $"/img/gemini-img-lemon-320.png" // Placeholder image URL, replace with actual logic if needed
        });
    }

    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products.Where(x => x.Id == request.Id)
                       .FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : new ProductDto
        {
            Id = data.Id,
            Name = data.Name,
            Description = data.Description,
            Price = data.Price
        };
    }
}