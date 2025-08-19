using ArtStore.Application.Features.Identity.DTOs;

namespace ArtStore.Application.Common.Interfaces.Identity;

public interface IRoleService
{
    List<ApplicationRoleDto> DataSource { get; }
    event Func<Task>? OnChange;
    void Initialize();
    void Refresh();
}