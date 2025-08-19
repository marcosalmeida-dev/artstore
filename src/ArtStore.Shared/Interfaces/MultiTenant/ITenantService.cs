using ArtStore.Shared.DTOs.Tenant;

namespace ArtStore.Shared.Interfaces.MultiTenant;

public interface ITenantService
{
    List<TenantDto> DataSource { get; }
    event Func<Task>? OnChange;
    void Initialize();
    void Refresh();
}