using System.Security.Claims;

namespace ArtStore.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface for setting the current user context.
/// </summary>
public interface ICurrentUserContextSetter
{
    /// <summary>
    /// Sets the current user from ClaimsPrincipal.
    /// </summary>
    /// <param name="principal">The claims principal representing the user.</param>
    void SetCurrentUser(ClaimsPrincipal principal);

    /// <summary>
    /// Sets the current user session information.
    /// </summary>
    /// <param name="sessionInfo">The session information to set.</param>
    void SetCurrentUser(SessionInfo sessionInfo);

    /// <summary>
    /// Clears the current user context.
    /// </summary>
    void ClearCurrentUser();
}
