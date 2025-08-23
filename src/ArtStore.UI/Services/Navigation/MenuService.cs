using ArtStore.Infrastructure.Constants.Role;
using ArtStore.UI.Models.NavigationMenu;

namespace ArtStore.UI.Services.Navigation;

public class MenuService : IMenuService
{
    private readonly List<MenuSectionModel> _features = new()
    {
        new MenuSectionModel
        {
            Title = "Application",
            SectionItems = new List<MenuSectionItemModel>
            {
                new() { Title = "Home", Icon = Icons.Material.Filled.Home, Href = "/" },
                new()
                {
                    Title = "E-Commerce",
                    Icon = Icons.Material.Filled.ShoppingCart,
                    PageStatus = PageStatus.Completed,
                    IsParent = true,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Products",
                            Href = "/pages/products",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Documents",
                            Href = "/pages/documents",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Contacts",
                            Href = "/pages/contacts",
                            PageStatus = PageStatus.Completed
                        }
                    }
                },
                new()
                {
                    Title = "Analytics",
                    Roles = new[] { RoleName.Admin, RoleName.Users },
                    Icon = Icons.Material.Filled.Analytics,
                    Href = "/analytics",
                    PageStatus = PageStatus.ComingSoon
                },
                new()
                {
                    Title = "Banking",
                    Roles = new[] { RoleName.Admin, RoleName.Users },
                    Icon = Icons.Material.Filled.Money,
                    Href = "/banking",
                    PageStatus = PageStatus.ComingSoon
                },
                new()
                {
                    Title = "Booking",
                    Roles = new[] { RoleName.Admin, RoleName.Users },
                    Icon = Icons.Material.Filled.CalendarToday,
                    Href = "/booking",
                    PageStatus = PageStatus.ComingSoon
                }
            }
        },
        new MenuSectionModel
        {
            Title = "ADMIN DASHBOARD",
            Roles = new[] { RoleName.Admin },
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    Title = "Dashboard",
                    Icon = Icons.Material.Filled.Dashboard,
                    Href = "/admin",
                    PageStatus = PageStatus.Completed
                }
            }
        },
        new MenuSectionModel
        {
            Title = "USER MANAGEMENT",
            Roles = new[] { RoleName.Admin },
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    IsParent = true,
                    Title = "Users & Roles",
                    Icon = Icons.Material.Filled.ManageAccounts,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Users",
                            Href = "/admin/users",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Roles",
                            Href = "/admin/roles",
                            PageStatus = PageStatus.Completed
                        }
                    }
                }
            }
        },
        new MenuSectionModel
        {
            Title = "PRODUCT MANAGEMENT",
            Roles = new[] { RoleName.Admin },
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    IsParent = true,
                    Title = "Products",
                    Icon = Icons.Material.Filled.Inventory,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Products",
                            Href = "/admin/products",
                            PageStatus = PageStatus.Completed
                        },
                        new()
                        {
                            Title = "Categories",
                            Href = "/admin/categories",
                            PageStatus = PageStatus.New
                        }
                    }
                }
            }
        },
        new MenuSectionModel
        {
            Title = "ORDER MANAGEMENT",
            Roles = new[] { RoleName.Admin },
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    IsParent = true,
                    Title = "Orders",
                    Icon = Icons.Material.Filled.ShoppingBag,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Orders",
                            Href = "/admin/orders",
                            PageStatus = PageStatus.New
                        },
                        new()
                        {
                            Title = "Order Status",
                            Href = "/admin/order-status",
                            PageStatus = PageStatus.New
                        }
                    }
                }
            }
        },
        new MenuSectionModel
        {
            Title = "SYSTEM",
            Roles = new[] { RoleName.Admin },
            SectionItems = new List<MenuSectionItemModel>
            {
                new()
                {
                    IsParent = true,
                    Title = "System Management",
                    Icon = Icons.Material.Filled.Settings,
                    MenuItems = new List<MenuSectionSubItemModel>
                    {
                        new()
                        {
                            Title = "Audit Trails",
                            Href = "/admin/audit-trails",
                            PageStatus = PageStatus.New
                        },
                        new()
                        {
                            Title = "System Logs",
                            Href = "/admin/logs",
                            PageStatus = PageStatus.New
                        },
                        new()
                        {
                            Title = "Tenants",
                            Href = "/admin/tenants",
                            PageStatus = PageStatus.New
                        }
                    }
                }
            }
        }
    };

    public IEnumerable<MenuSectionModel> Features => _features;
}