using ArtStore.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace ArtStore.Application.Common.Security;

public class UserProfileStateService : IDisposable
{
    // Internal user profile state
    private UserProfile _userProfile = new UserProfile { Email = "", UserId = "", UserName = "" };

    // Dependencies
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly HybridCache _cache;
    private readonly IServiceScope _scope;

    public UserProfileStateService(
        IServiceScopeFactory scopeFactory,
        HybridCache cache)
    {
        _scope = scopeFactory.CreateScope();
        _userManager = _scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        _cache = cache;
    }

    /// <summary>
    /// Loads and initializes the user profile from the database.
    /// </summary>
    public async Task InitializeAsync(string userName)
    {
        var key = GetApplicationUserCacheKey(userName);
        var result = await _cache.GetOrCreateAsync(
            key,
            async cancel => await _userManager.Users
                        .Where(x => x.UserName == userName)
                        .Include(x => x.UserRoles).ThenInclude(x => x.Role)
                        .FirstOrDefaultAsync(cancel),
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromSeconds(60),
                LocalCacheExpiration = TimeSpan.FromSeconds(30)
            });

        if (result is not null)
        {
            _userProfile = new UserProfile
            {
                UserId = result.Id,
                UserName = result.UserName ?? string.Empty,
                Email = result.Email ?? string.Empty,
                DisplayName = result.DisplayName ?? string.Empty,
                ProfilePictureDataUrl = result.ProfilePictureDataUrl,
                PhoneNumber = result.PhoneNumber,
                TimeZoneId = result.TimeZoneId,
                LanguageCode = result.LanguageCode
            };
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Gets or sets the current user profile.
    /// </summary>
    public UserProfile UserProfile
    {
        get => _userProfile;
        set
        {
            _userProfile = value;
            NotifyStateChanged();
        }
    }

    /// <summary>
    /// Refreshes the user profile by removing the cached value and reloading data from the database.
    /// </summary>
    public async Task RefreshAsync(string userName)
    {
        RemoveApplicationUserCache(userName);
        await InitializeAsync(userName);
    }

    /// <summary>
    /// Updates the user profile and clears the cache.
    /// </summary>
    public void UpdateUserProfile(string userName, string? profilePictureDataUrl, string? fullName, string? phoneNumber, string? timeZoneId, string? languageCode)
    {
        _userProfile.ProfilePictureDataUrl = profilePictureDataUrl;
        _userProfile.DisplayName = fullName;
        _userProfile.PhoneNumber = phoneNumber;
        _userProfile.TimeZoneId = timeZoneId;
        _userProfile.LanguageCode = languageCode;
        RemoveApplicationUserCache(userName);
        NotifyStateChanged();
    }

    public event Func<Task>? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();

    private string GetApplicationUserCacheKey(string userName)
    {
        return $"GetApplicationUserDto:{userName}";
    }

    public async void RemoveApplicationUserCache(string userName)
    {
        await _cache.RemoveAsync(GetApplicationUserCacheKey(userName));
    }

    public void Dispose() => _scope.Dispose();
}