namespace ArtStore.Infrastructure.Services.Identity;

/// <summary>
/// Represents the current user context, holding session information.
/// Uses AsyncLocal for async/await context flow.
/// </summary>
public class CurrentUserContext : ICurrentUserContext
{
    private static readonly AsyncLocal<SessionInfo?> _sessionInfo = new();

    /// <summary>
    /// Gets or sets the session information of the current user.
    /// </summary>
    public SessionInfo? SessionInfo
    {
        get => _sessionInfo.Value;
        set => _sessionInfo.Value = value;
    }
}
