using ArtStore.Application.Features.Tenants.Caching;
using ArtStore.Shared.DTOs.Tenant;
using ArtStore.Shared.Interfaces.MultiTenant;
using Microsoft.Extensions.Caching.Hybrid;

namespace ArtStore.Infrastructure.Services.MultiTenant;

public class TenantService : ITenantService
{
    private readonly IApplicationDbContext _context;
    private readonly HybridCache _cache;

    public TenantService(
        HybridCache cache,
        IServiceScopeFactory scopeFactory)
    {
        var scope = scopeFactory.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
        _cache = cache;
    }

    public event Func<Task>? OnChange;
    public List<TenantDto> DataSource { get; private set; } = new();


    public async void Initialize()
    {
        DataSource = await _cache.GetOrCreateAsync(
            TenantCacheKey.TenantsCacheKey,
            async cancel => await _context.Tenants
                .Select(s => new TenantDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                })
                .OrderBy(x => x.Name)
                .ToListAsync(cancel)) ?? new List<TenantDto>();
    }

    public async void Refresh()
    {
        await _cache.RemoveAsync(TenantCacheKey.TenantsCacheKey);
        DataSource = await _cache.GetOrCreateAsync(
            TenantCacheKey.TenantsCacheKey,
            async cancel => await _context.Tenants
                .Select(s => new TenantDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description
                })
                .OrderBy(x => x.Name)
                .ToListAsync(cancel)) ?? new List<TenantDto>();
        OnChange?.Invoke();
    }
}