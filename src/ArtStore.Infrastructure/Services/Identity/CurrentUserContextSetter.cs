using System.Security.Claims;
using ArtStore.Infrastructure.Constants.ClaimTypes;
using Microsoft.AspNetCore.Http;

namespace ArtStore.Infrastructure.Services.Identity;

/// <summary>
/// Service for setting the current user context.
/// </summary>
public class CurrentUserContextSetter : ICurrentUserContextSetter
{
    private readonly ICurrentUserContext _currentUserContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserContextSetter(
        ICurrentUserContext currentUserContext,
        IHttpContextAccessor httpContextAccessor)
    {
        _currentUserContext = currentUserContext;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Sets the current user from ClaimsPrincipal.
    /// </summary>
    public void SetCurrentUser(ClaimsPrincipal principal)
    {
        if (principal?.Identity?.IsAuthenticated != true)
        {
            ClearCurrentUser();
            return;
        }

        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
        var displayName = principal.FindFirst("DisplayName")?.Value ?? userName;
        var tenantId = principal.FindFirst(ApplicationClaimTypes.TenantId)?.Value;
        var profilePictureDataUrl = principal.FindFirst(ApplicationClaimTypes.ProfilePictureDataUrl)?.Value;

        // Get IP address from HttpContext
        var ipAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

        var sessionInfo = new SessionInfo(
            userId: userId,
            userName: userName,
            displayName: displayName,
            ipAddress: ipAddress,
            tenantId: tenantId,
            profilePictureDataUrl: profilePictureDataUrl,
            status: UserPresence.Available
        );

        _currentUserContext.SessionInfo = sessionInfo;
    }

    /// <summary>
    /// Sets the current user session information directly.
    /// </summary>
    public void SetCurrentUser(SessionInfo sessionInfo)
    {
        _currentUserContext.SessionInfo = sessionInfo;
    }

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    public void ClearCurrentUser()
    {
        _currentUserContext.SessionInfo = null;
    }
}
