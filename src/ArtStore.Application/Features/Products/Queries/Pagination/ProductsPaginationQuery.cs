using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Products.Caching;
using ArtStore.Application.Features.Products.Specifications;
using ArtStore.Shared.DTOs.Product;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Products.Queries.Pagination;

public class ProductsWithPaginationQuery : ProductAdvancedFilter, IQuery<PaginatedData<ProductDto>>
{
    public ProductAdvancedSpecification Specification => new(this);
    public string CacheKey => ProductCacheKey.GetSearchCacheKey($"{this}");

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
    public ProductsWithPaginationQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<PaginatedData<ProductDto>> Handle(ProductsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        // OrderBy extension applied before spec evaluation
        var query = _context.Products.OrderBy($"{request.OrderBy} {request.SortDirection}");

        var data = await query.PaginatedDataAsync(
            request.Specification,
            product => new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                // ... add all necessary mappings
            },
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return data;
    }
}