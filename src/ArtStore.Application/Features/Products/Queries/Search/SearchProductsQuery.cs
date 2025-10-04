using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Products.Caching;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.Extensions.Caching.Hybrid;

namespace ArtStore.Application.Features.Products.Queries.Search;

public class SearchProductsQuery : IQuery<PaginatedData<ProductDto>>
{
    public string? SearchString { get; set; }
    public bool? IsActive { get; set; }
    public int? CategoryId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? OrderBy { get; set; } = "Name";
    public string? SortDirection { get; set; } = "asc";

    public string CacheKey => ProductCacheKey.GetSearchCacheKey($"{SearchString}-{IsActive}-{CategoryId}-{PageNumber}-{PageSize}-{OrderBy}-{SortDirection}");
    public IEnumerable<string>? Tags => ProductCacheKey.Tags;
}

public class SearchProductsQueryHandler : IQueryHandler<SearchProductsQuery, PaginatedData<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly HybridCache _cache;

    public SearchProductsQueryHandler(IApplicationDbContext context, HybridCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<PaginatedData<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        var cacheKey = ProductCacheKey.GetSearchCacheKey($"{request.SearchString}-{request.IsActive}-{request.CategoryId}-{request.PageNumber}-{request.PageSize}-{request.OrderBy}-{request.SortDirection}");

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                var query = _context.Products
                    .Include(p => p.Category)
                    .Include(p => p.Tenant)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchString))
                {
                    query = query.Where(p => p.Name.Contains(request.SearchString) ||
                                           p.Description!.Contains(request.SearchString) ||
                                           p.Brand!.Contains(request.SearchString) ||
                                           p.ProductCode.Contains(request.SearchString));
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == request.IsActive.Value);
                }

                if (request.CategoryId.HasValue)
                {
                    query = query.Where(p => p.CategoryId == request.CategoryId.Value);
                }

                var totalCount = await query.CountAsync(cancel);

                // Apply sorting
                var sortBy = request.OrderBy?.ToLowerInvariant() ?? "name";
                var ascending = request.SortDirection?.ToLowerInvariant() != "desc";

                query = sortBy switch
                {
                    "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    "description" => ascending ? query.OrderBy(p => p.Description) : query.OrderByDescending(p => p.Description),
                    "brand" => ascending ? query.OrderBy(p => p.Brand) : query.OrderByDescending(p => p.Brand),
                    "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                    "productcode" => ascending ? query.OrderBy(p => p.ProductCode) : query.OrderByDescending(p => p.ProductCode),
                    "isactive" => ascending ? query.OrderBy(p => p.IsActive) : query.OrderByDescending(p => p.IsActive),
                    "categoryname" => ascending ? query.OrderBy(p => p.Category!.Name) : query.OrderByDescending(p => p.Category!.Name),
                    "created" => ascending ? query.OrderBy(p => p.Created) : query.OrderByDescending(p => p.Created),
                    _ => query.OrderBy(p => p.Name)
                };

                var products = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(x => new ProductDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Brand = x.Brand,
                        Unit = x.Unit,
                        Price = x.Price,
                        IsActive = x.IsActive,
                        CategoryId = x.CategoryId,
                        CategoryName = x.Category!.Name,
                        TenantId = x.TenantId,
                        TenantName = x.Tenant!.Name,
                        ProductCode = x.ProductCode,
                        Created = x.Created,
                        LastModified = x.LastModified
                    })
                    .ToListAsync(cancel);

                return new PaginatedData<ProductDto>(products, totalCount, request.PageNumber, request.PageSize);
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(10),
                LocalCacheExpiration = TimeSpan.FromMinutes(3)
            },
            cancellationToken: cancellationToken);
    }
}