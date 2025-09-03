using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Products.Caching;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Products.Queries.GetAll;

public class GetAllProductsQuery : IQuery<IEnumerable<ProductDto>>
{
    public string CacheKey => ProductCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
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

    public async Task<IEnumerable<ProductDto?>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Tenant)
            .ToListAsync(cancellationToken);

        return data.Select(x => new ProductDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Brand = x.Brand,
            Unit = x.Unit,
            Price = x.Price,
            IsActive = x.IsActive,
            CategoryId = x.CategoryId,
            CategoryName = x.Category?.Name,
            TenantId = x.TenantId,
            TenantName = x.Tenant?.Name,
            Pictures = x.Pictures?.Select(p => new ProductImageDto
            {
                Name = p.Name,
                Size = p.Size,
                Url = p.Url
            }).ToList() ?? new List<ProductImageDto>(),
            Created = x.Created,
            LastModified = x.LastModified
        });
    }

    public async Task<ProductDto?> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Tenant)
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return data == null ? null : new ProductDto
        {
            Id = data.Id,
            Name = data.Name,
            Description = data.Description,
            Brand = data.Brand,
            Unit = data.Unit,
            Price = data.Price,
            IsActive = data.IsActive,
            CategoryId = data.CategoryId,
            CategoryName = data.Category?.Name,
            TenantId = data.TenantId,
            TenantName = data.Tenant?.Name,
            Pictures = data.Pictures?.Select(p => new ProductImageDto
            {
                Name = p.Name,
                Size = p.Size,
                Url = p.Url
            }).ToList() ?? new List<ProductImageDto>(),
            Created = data.Created,
            LastModified = data.LastModified
        };
    }
}