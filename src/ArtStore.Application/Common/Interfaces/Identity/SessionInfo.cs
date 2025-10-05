namespace ArtStore.Application.Common.Interfaces.Identity;

/// <summary>
/// Represents user session information.
/// Using record for immutability and value-based equality.
/// </summary>
public sealed record SessionInfo
{
    public string? UserId { get; init; }
    public string? UserName { get; init; }
    public string? DisplayName { get; init; }
    public string? IPAddress { get; init; }
    public string? TenantId { get; init; }
    public string? ProfilePictureDataUrl { get; init; }
    public UserPresence Status { get; init; }

    public SessionInfo()
    {
        Status = UserPresence.Statusunknown;
    }

    public SessionInfo(
        string? userId,
        string? userName,
        string? displayName,
        string? ipAddress,
        string? tenantId,
        string? profilePictureDataUrl,
        UserPresence status)
    {
        UserId = userId;
        UserName = userName;
        DisplayName = displayName;
        IPAddress = ipAddress;
        TenantId = tenantId;
        ProfilePictureDataUrl = profilePictureDataUrl;
        Status = status;
    }
}

public enum UserPresence
{
    Available,
    Busy,
    Donotdisturb,
    Away,
    Offline,
    Statusunknown
}