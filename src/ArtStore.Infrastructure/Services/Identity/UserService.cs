using ArtStore.Application.Features.Identity.DTOs;
using ArtStore.Domain.Identity;
using Microsoft.Extensions.Caching.Hybrid;

namespace ArtStore.Infrastructure.Services.Identity;

public class UserService : IUserService
{
    private const string CACHEKEY = "ALL-ApplicationUserDto";
    private readonly HybridCache _cache;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(
        HybridCache cache,
        IServiceScopeFactory scopeFactory)
    {
        _cache = cache;
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        DataSource = new List<ApplicationUserDto>();
    }

    public List<ApplicationUserDto> DataSource { get; private set; }

    public event Func<Task>? OnChange;

    public async void Initialize()
    {
        DataSource = await _cache.GetOrCreateAsync(
                         CACHEKEY,
                         async cancel => await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                             .Select(s => new ApplicationUserDto
                             {
                                 Id = s.Id,
                                 UserName = s.UserName,
                                 Email = s.Email,
                                 DisplayName = s.DisplayName,
                                 IsActive = s.IsActive,
                                 IsLive = s.IsLive,
                                 ProfilePictureDataUrl = s.ProfilePictureDataUrl,
                                 TenantId = s.TenantId
                             })
                             .OrderBy(x => x.UserName)
                             .ToListAsync(cancel))
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }


    public async void Refresh()
    {
        await _cache.RemoveAsync(CACHEKEY);
        DataSource = await _cache.GetOrCreateAsync(
                         CACHEKEY,
                         async cancel => await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                             .Select(s => new ApplicationUserDto
                             {
                                 Id = s.Id,
                                 UserName = s.UserName,
                                 Email = s.Email,
                                 DisplayName = s.DisplayName,
                                 IsActive = s.IsActive,
                                 IsLive = s.IsLive,
                                 ProfilePictureDataUrl = s.ProfilePictureDataUrl,
                                 TenantId = s.TenantId
                             })
                             .OrderBy(x => x.UserName)
                             .ToListAsync(cancel))
                     ?? new List<ApplicationUserDto>();
        OnChange?.Invoke();
    }
}