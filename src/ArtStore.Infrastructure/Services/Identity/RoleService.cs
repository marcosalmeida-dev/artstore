using ArtStore.Application.Features.Identity.DTOs;
using ArtStore.Domain.Identity;
using Microsoft.Extensions.Caching.Hybrid;

namespace ArtStore.Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private const string CACHEKEY = "ALL-ApplicationRoleDto";
    private readonly HybridCache _cache;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(
        HybridCache cache,
        IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        var scope = scopeFactory.CreateScope();
        _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        DataSource = new List<ApplicationRoleDto>();
    }

    public List<ApplicationRoleDto> DataSource { get; private set; }

    public event Func<Task>? OnChange;

    public async void Initialize()
    {
        DataSource = await _cache.GetOrCreateAsync(
                         CACHEKEY,
                         async cancel => await Task.Run(() => _roleManager.Roles
                             .Select(s => new ApplicationRoleDto()
                             {
                                 Id = s.Id,
                                 Name = s.Name,
                                 NormalizedName = s.NormalizedName,
                                 Description = s.Description,
                                 TenantId = s.TenantId
                             }).OrderBy(x => x.TenantId).ThenBy(x => x.Name)
                             .ToList(), cancel))
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }


    public async void Refresh()
    {
        await _cache.RemoveAsync(CACHEKEY);
        DataSource = await _cache.GetOrCreateAsync(
                         CACHEKEY,
                         async cancel => await Task.Run(() => _roleManager.Roles
                             .Select(s => new ApplicationRoleDto()
                             {
                                 Id = s.Id,
                                 Name = s.Name,
                                 NormalizedName = s.NormalizedName,
                                 Description = s.Description,
                                 TenantId = s.TenantId
                             })
                             .OrderBy(x => x.TenantId).ThenBy(x => x.Name)
                             .ToList(), cancel))
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }
}