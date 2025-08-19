using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Tenants.Caching;
using ArtStore.Shared.DTOs.Tenant;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Tenants.Queries.Pagination;

public class TenantsWithPaginationQuery : PaginationFilter, IQuery<PaginatedData<TenantDto>>
{
    public TenantsPaginationSpecification Specification => new(this);
    public string CacheKey => TenantCacheKey.GetPaginationCacheKey($"{this}");
    public IEnumerable<string>? Tags => TenantCacheKey.Tags;

    public override string ToString()
    {
        return $"Search:{Keyword},OrderBy:{OrderBy} {SortDirection},{PageNumber},{PageSize}";
    }
}

public class TenantsWithPaginationQueryHandler :
    IQueryHandler<TenantsWithPaginationQuery, PaginatedData<TenantDto>?>
{
    private readonly IApplicationDbContext _context;

    public TenantsWithPaginationQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<PaginatedData<TenantDto>> Handle(TenantsWithPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.Tenants.OrderBy($"{request.OrderBy} {request.SortDirection}");

        var data = await query.PaginatedDataAsync(
            request.Specification,
            product => new TenantDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description
            },
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return data;
    }
}
#nullable disable warnings
public class TenantsPaginationSpecification : Specification<Tenant>
{
    public TenantsPaginationSpecification(TenantsWithPaginationQuery query)
    {
        Query.Where(q => q.Name != null)
            .Where(q => q.Name.Contains(query.Keyword) || q.Description.Contains(query.Keyword),
                !string.IsNullOrEmpty(query.Keyword));
    }
}