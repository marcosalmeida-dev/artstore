using ArtStore.Application.Features.Tenants.Caching;
using ArtStore.Shared.DTOs.Tenant;
using ArtStore.Shared.Interfaces.MultiTenant;
using ZiggyCreatures.Caching.Fusion;

namespace ArtStore.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _context;
    private readonly IFusionCache _fusionCache;

    public TenantService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _fusionCache = fusionCache;
    }

    public event Func<Task>? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();


    public void Initialize()
    {
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => _context.Tenants
                .Select(s => new TenantDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                })
                .OrderBy(x => x.Name)
                .ToList()) ?? new List<TenantDto>();
    }

    public void Refresh()
    {
        _fusionCache.Remove(TenantCacheKey.TenantsCacheKey);
        DataSource = _fusionCache.GetOrSet(TenantCacheKey.TenantsCacheKey,
            _ => _context.Tenants
                .Select(s => new TenantDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                })
                .OrderBy(x => x.Name)
                .ToList()) ?? new List<TenantDto>();
        OnChange?.Invoke();
    }
}