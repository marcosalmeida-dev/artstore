using ArtStore.Application.Features.Identity.DTOs;

namespace ArtStore.Application.Common.Interfaces.Identity;

public interface IUserService
{
    List<ApplicationUserDto> DataSource { get; }
    event Func<Task>? OnChange;
    void Initialize();
    void Refresh();
}