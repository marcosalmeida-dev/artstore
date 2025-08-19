using ArtStore.Application.Features.Identity.DTOs;
using ArtStore.Domain.Identity;
using ZiggyCreatures.Caching.Fusion;

namespace ArtStore.Infrastructure.Services.Identity;

public class RoleService : IRoleService
{
    private const string CACHEKEY = "ALL-ApplicationRoleDto";
    private readonly IFusionCache _fusionCache;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public RoleService(
        IFusionCache fusionCache,
        IServiceScopeFactory scopeFactory)
    {
        _fusionCache = fusionCache;
        var scope = scopeFactory.CreateScope();
        _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        DataSource = new List<ApplicationRoleDto>();
    }

    public List<ApplicationRoleDto> DataSource { get; private set; }

    public event Func<Task>? OnChange;

    public void Initialize()
    {
        DataSource = _fusionCache.GetOrSet(CACHEKEY,
                         _ => _roleManager.Roles
                             .Select(s => new ApplicationRoleDto()
                             {
                                    Id = s.Id,
                                    Name = s.Name,
                                    NormalizedName = s.NormalizedName,
                                    Description = s.Description,
                                    TenantId = s.TenantId
                             }).OrderBy(x => x.TenantId).ThenBy(x => x.Name)
                             .ToList())
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }


    public void Refresh()
    {
        _fusionCache.Remove(CACHEKEY);
        DataSource = _fusionCache.GetOrSet(CACHEKEY,
                         _ => _roleManager.Roles
                             .Select(s => new ApplicationRoleDto()
                             {
                                 Id = s.Id,
                                 Name = s.Name,
                                 NormalizedName = s.NormalizedName,
                                 Description = s.Description,
                                 TenantId = s.TenantId
                             })
                             .OrderBy(x => x.TenantId).ThenBy(x => x.Name)
                             .ToList())
                     ?? new List<ApplicationRoleDto>();
        OnChange?.Invoke();
    }
}