using ArtStore.UI.Models.NavigationMenu;

namespace ArtStore.UI.Services.Navigation;

public interface IMenuService
{
    IEnumerable<MenuSectionModel> Features { get; }
}