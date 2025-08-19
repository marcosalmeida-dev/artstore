using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Tenants.Caching;
using ArtStore.Shared.DTOs.Tenant;
using ArtStore.Shared.Interfaces.Query;

namespace ArtStore.Application.Features.Tenants.Queries.GetAll;

public class GetAllTenantsQuery : IQuery<IEnumerable<TenantDto>>
{
    public string CacheKey => TenantCacheKey.GetAllCacheKey;
    public IEnumerable<string>? Tags => TenantCacheKey.Tags;
}

public class GetAllTenantsQueryHandler :
    IQueryHandler<GetAllTenantsQuery, IEnumerable<TenantDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAllTenantsQueryHandler(
        IApplicationDbContext context
    )
    {
        _context = context;
    }

    public async Task<IEnumerable<TenantDto>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
    {
        var data = await _context.Tenants
            .ToListAsync(cancellationToken);

        return data.Select(x => new TenantDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description
        });
    }
}