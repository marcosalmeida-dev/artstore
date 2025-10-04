using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Products.Caching;
using ArtStore.Application.Features.Products.Specifications;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.Extensions.Caching.Hybrid;

namespace ArtStore.Application.Features.Products.Queries.Pagination;

public class ProductsWithPaginationQuery : ProductAdvancedFilter, IQuery<PaginatedData<ProductDto>>
{
    public ProductAdvancedSpecification Specification => new(this);
    public string CacheKey => ProductCacheKey.GetPaginationCacheKey($"{this}");

    public IEnumerable<string>? Tags => ProductCacheKey.Tags;

    // the currently logged in user
    public override string ToString()
    {
        return
            $"CurrentUser:{CurrentUser?.UserId},ListView:{ListView},Search:{Keyword},Name:{Name},Brand:{Brand},Unit:{Unit},MinPrice:{MinPrice},MaxPrice:{MaxPrice},SortDirection:{SortDirection},OrderBy:{OrderBy},{PageNumber},{PageSize}";
    }
}

public class ProductsWithPaginationQueryHandler :
    IQueryHandler<ProductsWithPaginationQuery, PaginatedData<ProductDto>?>
{
    private readonly IApplicationDbContext _context;
    private readonly HybridCache _cache;

    public ProductsWithPaginationQueryHandler(
        IApplicationDbContext context,
        HybridCache cache
    )
    {
        _context = context;
        _cache = cache;
    }

    public async Task<PaginatedData<ProductDto>> Handle(ProductsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = ProductCacheKey.GetPaginationCacheKey($"{request}");

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                // OrderBy extension applied before spec evaluation
                var query = _context.Products.OrderBy($"{request.OrderBy} {request.SortDirection}");

                var data = await query.PaginatedDataAsync(
                    request.Specification,
                    product => new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Description,
                        Brand = product.Brand,
                        Unit = product.Unit,
                        Price = product.Price,
                        IsActive = product.IsActive,
                        CategoryId = product.CategoryId,
                        ProductCode = product.ProductCode,
                        Created = product.Created,
                        LastModified = product.LastModified
                    },
                    request.PageNumber,
                    request.PageSize,
                    cancel);

                return data;
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(3)
            },
            cancellationToken: cancellationToken);
    }
}