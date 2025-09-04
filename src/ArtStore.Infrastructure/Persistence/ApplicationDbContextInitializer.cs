using System.Reflection;
using ArtStore.Domain.Entities.Translations;
using ArtStore.Domain.Extensions;
using ArtStore.Domain.Identity;
using ArtStore.Infrastructure.Constants.ClaimTypes;
using ArtStore.Infrastructure.Constants.Role;
using ArtStore.Infrastructure.Constants.User;
using ArtStore.Infrastructure.PermissionSet;

namespace ArtStore.Infrastructure.Persistence;

public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsRelational())
            {
                await _context.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedTenantsAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedDataAsync();
            await SeedCategoriesAndProductsAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static IEnumerable<string> GetAllPermissions()
    {
        var allPermissions = new List<string>();
        var modules = typeof(Permissions).GetNestedTypes();

        foreach (var module in modules)
        {
            var moduleName = string.Empty;
            var moduleDescription = string.Empty;

            var fields = module.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            foreach (var fi in fields)
            {
                var propertyValue = fi.GetValue(null);

                if (propertyValue is not null)
                {
                    allPermissions.Add((string)propertyValue);
                }
            }
        }

        return allPermissions;
    }

    private async Task SeedTenantsAsync()
    {
        if (await _context.Tenants.AnyAsync())
        {
            return;
        }

        _logger.LogInformation("Seeding tenants...");
        var tenants = new[]
        {
                new Tenant { Name = "Master", Description = "Master Site" },
                new Tenant { Name = "Slave", Description = "Slave Site" }
        };

        await _context.Tenants.AddRangeAsync(tenants);
        await _context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        var adminRoleName = RoleName.Admin;
        var userRoleName = RoleName.Basic;

        if (await _roleManager.RoleExistsAsync(adminRoleName))
        {
            return;
        }

        _logger.LogInformation("Seeding roles...");
        var administratorRole = new ApplicationRole(adminRoleName)
        {
            Description = "Admin Group",
            TenantId = (await _context.Tenants.FirstAsync()).Id
        };
        var userRole = new ApplicationRole(userRoleName)
        {
            Description = "Basic Group",
            TenantId = (await _context.Tenants.FirstAsync()).Id
        };

        await _roleManager.CreateAsync(administratorRole);
        await _roleManager.CreateAsync(userRole);

        var permissions = GetAllPermissions();

        foreach (var permission in permissions)
        {
            var claim = new Claim(ApplicationClaimTypes.Permission, permission);
            await _roleManager.AddClaimAsync(administratorRole, claim);

            if (permission.StartsWith("Permissions.Products"))
            {
                await _roleManager.AddClaimAsync(userRole, claim);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.Users.AnyAsync())
        {
            return;
        }

        _logger.LogInformation("Seeding users...");
        var adminUser = new ApplicationUser
        {
            UserName = "admin@test.com",
            Provider = "Local",
            IsActive = true,
            TenantId = (await _context.Tenants.FirstAsync()).Id,
            DisplayName = UserName.Administrator,
            Email = "admin@test.com",
            EmailConfirmed = true,
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/78be68221020124c23c665ac54e07074?s=80",
            LanguageCode = "en-US",
            TimeZoneId = "Asia/Shanghai",
            TwoFactorEnabled = false
        };

        var demoUser = new ApplicationUser
        {
            UserName = UserName.Demo,
            IsActive = true,
            Provider = "Local",
            TenantId = (await _context.Tenants.FirstAsync()).Id,
            DisplayName = UserName.Demo,
            Email = "demo@example.com",
            EmailConfirmed = true,
            LanguageCode = "de-DE",
            TimeZoneId = "Europe/Berlin",
            ProfilePictureDataUrl = "https://s.gravatar.com/avatar/ea753b0b0f357a41491408307ade445e?s=80"
        };

        await _userManager.CreateAsync(adminUser, UserName.DefaultPassword);
        await _userManager.AddToRoleAsync(adminUser, RoleName.Admin);

        await _userManager.CreateAsync(demoUser, UserName.DefaultPassword);
        await _userManager.AddToRoleAsync(demoUser, RoleName.Basic);
    }

    private async Task SeedCategoriesAndProductsAsync()
    {
        if (await _context.Categories.AnyAsync() || await _context.Products.AnyAsync())
        {
            return;
        }

        _logger.LogInformation("Seeding categories and products...");

        var tenant = await _context.Tenants.FirstAsync(); // Assume at least one tenant exists

        var drinks = new Category
        {
            Name = "Drinks",
            Description = "Regular beverages",
            TenantId = tenant.Id,
            IsActive = true,
        };
        drinks.SetTranslation("pt-BR", "Bebidas", "Bebidas normais");
        drinks.SetTranslation("es-AR", "Bebidas", "Bebidas normales");

        var slushDrinks = new Category
        {
            Name = "Slush Drinks",
            Description = "Refreshing slush beverages",
            TenantId = tenant.Id,
            IsActive = true,
        };
        slushDrinks.SetTranslation("pt-BR", "Bebidas Geladas", "Bebidas refrescantes geladas");
        slushDrinks.SetTranslation("es-AR", "Bebidas Heladas", "Bebidas refrescantes heladas");

        var frozenDrinks = new Category
        {
            Name = "Frozen Drinks",
            Description = "Frozen beverages",
            TenantId = tenant.Id,
            IsActive = true,
        };
        frozenDrinks.SetTranslation("pt-BR", "Bebidas Congeladas", "Bebidas congeladas");
        frozenDrinks.SetTranslation("es-AR", "Bebidas Congeladas", "Bebidas congeladas");

        var savoryPastry = new Category
        {
            Name = "Savory Pastry",
            Description = "Freshly baked savory pastries",
            TenantId = tenant.Id,
            IsActive = true,
        };
        savoryPastry.SetTranslation("pt-BR", "Salgados", "Salgados frescos assados");
        savoryPastry.SetTranslation("es-AR", "Pastelería Salada", "Pastelería salada recién horneada");

        var categories = new[] { drinks, slushDrinks, frozenDrinks, savoryPastry };
        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();

        // Create products list
        var products = new List<Product>();

        // Sugarcane Juice flavors and their translations
        var flavors = new[]
        {
            ("Pure", "Puro", "Puro", 8.00m),
            ("Passion Fruit", "Maracujá", "Maracuyá", 9.50m),
            ("Strawberry", "Morango", "Fresa", 9.50m),
            ("Lemon", "Limão", "Limón", 9.00m),
            ("Açaí", "Açaí", "Açaí", 12.00m),
            ("Cocoa", "Cacau", "Cacao", 10.50m),
            ("Coffee", "Café", "Café", 10.00m),
            ("Detox", "Detox", "Detox", 11.50m)
        };

        // Categories and their preparation types
        var categoryTypes = new[]
        {
            (drinks, "Regular", "Normal", "Normal"),
            (slushDrinks, "Slush", "Gelada", "Helada"),
            (frozenDrinks, "Frozen", "Congelada", "Congelada")
        };

        // Generate all combinations
        foreach (var (category, type, typePtBr, typeEsAr) in categoryTypes)
        {
            foreach (var (flavorEn, flavorPtBr, flavorEsAr, basePrice) in flavors)
            {
                // Adjust price based on preparation type
                var price = type switch
                {
                    "Regular" => basePrice,
                    "Slush" => basePrice + 2.00m,
                    "Frozen" => basePrice + 3.50m,
                    _ => basePrice
                };

                var nutritionFacts = new NutritionFacts
                {
                    Calories = flavorEn switch
                    {
                        "Pure" => 112,
                        "Passion Fruit" => 125,
                        "Strawberry" => 128,
                        "Lemon" => 108,
                        "Açaí" => 160,
                        "Cocoa" => 145,
                        "Coffee" => 118,
                        "Detox" => 95,
                        _ => 112
                    },
                    Carbohydrates = flavorEn switch
                    {
                        "Pure" => 26,
                        "Açaí" => 32,
                        "Cocoa" => 28,
                        _ => 24
                    },
                    Sugar = flavorEn switch
                    {
                        "Pure" => 25,
                        "Detox" => 18,
                        _ => 22
                    },
                    Protein = 0.4m,
                    Fat = flavorEn == "Açaí" ? 2.1m : 0.1m,
                    Fiber = flavorEn switch
                    {
                        "Açaí" => 3.2m,
                        "Detox" => 2.8m,
                        _ => 0.5m
                    },
                    AdditionalNutrients = new Dictionary<string, decimal>
                    {
                        ["Vitamin C"] = flavorEn switch
                        {
                            "Pure" => 25,
                            "Passion Fruit" => 30,
                            "Strawberry" => 35,
                            "Lemon" => 45,
                            "Detox" => 40,
                            _ => 20
                        },
                        ["Iron"] = flavorEn == "Açaí" ? 1.2m : 0.5m,
                        ["Antioxidants"] = flavorEn switch
                        {
                            "Açaí" => 95,
                            "Detox" => 85,
                            "Cocoa" => 78,
                            _ => 35
                        }
                    }
                };

                var product = new Product
                {
                    Name = $"{type} Sugarcane Juice - {flavorEn}",
                    Description = $"Fresh {type.ToLower()} sugarcane juice with {flavorEn.ToLower()} flavor",
                    Brand = "Cana Brasil",
                    Unit = "300ml Cup",
                    Price = price,
                    TenantId = tenant.Id,
                    Category = category,
                    Pictures = new List<ProductImage>
                    {
                        new ProductImage
                        {
                            Name = $"cana-{type.ToLower()}-{flavorEn.ToLower().Replace(" ", "-")}.jpg",
                            Url = $"/images/products/cana-{type.ToLower()}-{flavorEn.ToLower().Replace(" ", "-")}.jpg",
                            Size = 200
                        }
                    }
                };

                // Add translations
                product.SetTranslation("pt-BR",
                    $"Caldo de Cana {typePtBr} - {flavorPtBr}",
                    $"Caldo de cana fresco {typePtBr.ToLower()} com sabor {flavorPtBr.ToLower()}",
                    "Copo 300ml",
                    nutritionFacts);

                product.SetTranslation("es-AR",
                    $"Jugo de Caña {typeEsAr} - {flavorEsAr}",
                    $"Jugo de caña fresco {typeEsAr.ToLower()} con sabor {flavorEsAr.ToLower()}",
                    "Vaso 300ml",
                    nutritionFacts);

                products.Add(product);
            }
        }

        // Add some savory pastries
        var pastryProducts = new[]
        {
            new Product
            {
                Name = "Cheese Pastry",
                Description = "Crispy pastry filled with melted cheese",
                Brand = "Salgados da Casa",
                Unit = "Piece",
                Price = 6.50m,
                TenantId = tenant.Id,
                Category = savoryPastry,
                Pictures = new List<ProductImage>
                {
                    new ProductImage { Name = "pastel-queijo.jpg", Url = "/images/products/pastel-queijo.jpg", Size = 180 }
                }
            },
            new Product
            {
                Name = "Meat Pastry",
                Description = "Traditional pastry with seasoned ground beef",
                Brand = "Salgados da Casa",
                Unit = "Piece",
                Price = 7.50m,
                TenantId = tenant.Id,
                Category = savoryPastry,
                Pictures = new List<ProductImage>
                {
                    new ProductImage { Name = "pastel-carne.jpg", Url = "/images/products/pastel-carne.jpg", Size = 180 }
                }
            }
        };

        // Add translations to pastries
        pastryProducts[0].SetTranslation("pt-BR", "Pastel de Queijo", "Pastel crocante recheado com queijo derretido", "Unidade");
        pastryProducts[0].SetTranslation("es-AR", "Empanada de Queso", "Empanada crujiente rellena de queso derretido", "Unidad");

        pastryProducts[1].SetTranslation("pt-BR", "Pastel de Carne", "Pastel tradicional com carne moída temperada", "Unidade");
        pastryProducts[1].SetTranslation("es-AR", "Empanada de Carne", "Empanada tradicional con carne picada condimentada", "Unidad");

        products.AddRange(pastryProducts);

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
    }

    private async Task SeedDataAsync()
    {
        //if (!await _context.PicklistSets.AnyAsync())
        //{

        //    _logger.LogInformation("Seeding key values...");
        //    var keyValues = new[]
        //    {
        //        new PicklistSet
        //        {
        //            Name = Picklist.Status,
        //            Value = "initialization",
        //            Text = "Initialization",
        //            Description = "Status of workflow"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Status,
        //            Value = "processing",
        //            Text = "Processing",
        //            Description = "Status of workflow"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Status,
        //            Value = "pending",
        //            Text = "Pending",
        //            Description = "Status of workflow"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Status,
        //            Value = "done",
        //            Text = "Done",
        //            Description = "Status of workflow"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Brand,
        //            Value = "Apple",
        //            Text = "Apple",
        //            Description = "Brand of production"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Brand,
        //            Value = "Google",
        //            Text = "Google",
        //            Description = "Brand of production"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Brand,
        //            Value = "Microsoft",
        //            Text = "Microsoft",
        //            Description = "Brand of production"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Unit,
        //            Value = "EA",
        //            Text = "EA",
        //            Description = "Unit of product"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Unit,
        //            Value = "KM",
        //            Text = "KM",
        //            Description = "Unit of product"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Unit,
        //            Value = "PC",
        //            Text = "PC",
        //            Description = "Unit of product"
        //        },
        //        new PicklistSet
        //        {
        //            Name = Picklist.Unit,
        //            Value = "L",
        //            Text = "L",
        //            Description = "Unit of product"
        //        }
        //    };

        //    await _context.PicklistSets.AddRangeAsync(keyValues);
        //    await _context.SaveChangesAsync();
        //}

        //if (!await _context.Products.AnyAsync())
        //{

        //    _logger.LogInformation("Seeding products...");
        //    var products = new[]
        //    {
        //        new Product
        //        {
        //            Brand = "Apple",
        //            Name = "IPhone 13 Pro",
        //            Description =
        //            "Apple iPhone 13 Pro smartphone. Announced Sep 2021. Features 6.1″ display, Apple A15 Bionic chipset, 3095 mAh battery, 1024 GB storage.",
        //            Unit = "EA",
        //            Price = 999.98m
        //        },
        //        new Product
        //        {
        //            Brand = "Sony",
        //            Name = "WH-1000XM4",
        //            Description = "Sony WH-1000XM4 Wireless Noise-Canceling Over-Ear Headphones. Features industry-leading noise cancellation, up to 30 hours of battery life, touch sensor controls.",
        //            Unit = "EA",
        //            Price = 349.99m
        //        },
        //        new Product
        //        {
        //            Brand = "Nintendo",
        //            Name = "Switch OLED Model",
        //            Description = "Nintendo Switch OLED Model console. Released October 2021. Features 7″ OLED screen, 64GB internal storage, enhanced audio, dock with wired LAN port.",
        //            Unit = "EA",
        //            Price = 349.99m
        //        },
        //        new Product
        //        {
        //            Brand = "Apple",
        //            Name = "MacBook Air M1",
        //            Description = "Apple MacBook Air with M1 chip. Features 13.3″ Retina display, Apple M1 chip with 8‑core CPU, 8GB RAM, 256GB SSD storage, up to 18 hours of battery life.",
        //            Unit = "EA",
        //            Price = 999.99m
        //        }

        //    };

        //    await _context.Products.AddRangeAsync(products);
        //    await _context.SaveChangesAsync();
        //}
    }
}