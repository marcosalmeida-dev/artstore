using ArtStore.Application.Common.ExceptionHandlers;
using ArtStore.Application.Features.Identity.DTOs;
using ArtStore.Domain.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using ZiggyCreatures.Caching.Fusion;

namespace ArtStore.Infrastructure.Services.Identity;

public class IdentityService : IIdentityService
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IStringLocalizer<IdentityService> _localizer;
    private readonly IFusionCache _fusionCache;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
    private readonly UserManager<ApplicationUser> _userManager;

    public IdentityService(
        IServiceScopeFactory scopeFactory,
        IApplicationSettings appConfig,
        IFusionCache fusionCache,
        IStringLocalizer<IdentityService> localizer)
    {
        var scope = scopeFactory.CreateScope();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _userClaimsPrincipalFactory = scope.ServiceProvider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>();
        _authorizationService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        _fusionCache = fusionCache;
        _localizer = localizer;
    }

    private TimeSpan RefreshInterval => TimeSpan.FromMinutes(60);

    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellation = default)
    {
        var key = $"GetUserNameAsync:{userId}";
        var user = await _fusionCache.GetOrSetAsync(key,
             _ => _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId), RefreshInterval);
        return user?.UserName;
    }

    public string GetUserName(string userId)
    {
        var key = $"GetUserName-byId:{userId}";
        var user = _fusionCache.GetOrSet(key, _ => _userManager.Users.SingleOrDefault(u => u.Id == userId), RefreshInterval);
        return user?.UserName ?? string.Empty;
    }

    public async Task<bool> IsInRoleAsync(string userId, string role, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<bool> AuthorizeAsync(string userId, string policyName, CancellationToken cancellation = default)
    {
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId, cancellation) ??
                   throw new NotFoundException(_localizer["User Not Found."]);
        var principal = await _userClaimsPrincipalFactory.CreateAsync(user);
        var result = await _authorizationService.AuthorizeAsync(principal, policyName);
        return result.Succeeded;
    }

    public async Task<IDictionary<string, string?>> FetchUsers(string roleName,
        CancellationToken cancellation = default)
    {
        var result = await _userManager.Users
            .Where(x => x.UserRoles.Any(y => y.Role.Name == roleName))
            .Include(x => x.UserRoles)
            .ToDictionaryAsync(x => x.UserName!, y => y.DisplayName, cancellation);
        return result;
    }


    public async Task<ApplicationUserDto?> GetApplicationUserDto(string userName,
        CancellationToken cancellation = default)
    {
        var key = GetApplicationUserCacheKey(userName);
        var result = await _fusionCache.GetOrSetAsync(key,
            _ => _userManager.Users.Where(x => x.UserName == userName).Include(x => x.UserRoles)
                .FirstOrDefaultAsync(cancellation), RefreshInterval);
        return new ApplicationUserDto()
        {
            Id = result?.Id ?? string.Empty,
            UserName = result?.UserName ?? string.Empty,
            DisplayName = result?.DisplayName ?? string.Empty,
            Email = result?.Email ?? string.Empty,
            PhoneNumber = result?.PhoneNumber ?? string.Empty,
            TenantId = result?.TenantId
        };
    }

    public async Task<List<ApplicationUserDto>?> GetUsers(int? tenantId, CancellationToken cancellation = default)
    {
        var key = $"GetApplicationUserDtoListWithTenantId:{tenantId}";
        Func<int?, CancellationToken, Task<List<ApplicationUserDto>?>> getUsersByTenantId =
            async (tenantId, token) =>
            {
                if (tenantId.HasValue)
                {
                    return await _userManager.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role)
                    .Select(s => new ApplicationUserDto()
                    {
                        Id = s.Id,
                        UserName = s.UserName,
                        DisplayName = s.DisplayName,
                        Email = s.Email,
                        PhoneNumber = s.PhoneNumber,
                        TenantId = s.TenantId
                    }).ToListAsync();
                }
                return await _userManager.Users.Where(x => x.TenantId == tenantId).Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .Select(s => new ApplicationUserDto()
                    {
                        Id = s.Id,
                        UserName = s.UserName,
                        DisplayName = s.DisplayName,
                        Email = s.Email,
                        PhoneNumber = s.PhoneNumber,
                        TenantId = s.TenantId
                    }).ToListAsync();
            };
        var result = await _fusionCache.GetOrSetAsync(key, _ => getUsersByTenantId(tenantId, cancellation), RefreshInterval);
        return result;
    }

    public void RemoveApplicationUserCache(string userName)
    {
        _fusionCache.Remove(GetApplicationUserCacheKey(userName));
    }

    private string GetApplicationUserCacheKey(string userName)
    {
        return $"GetApplicationUserDto:{userName}";
    }
}