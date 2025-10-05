using System.Reflection;
using ArtStore.Domain.Entities.Enums;
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
            await SeedCategoriesAndProductsAsync();
            await SeedInventoryAsync();
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

        var iceSlushDrinks = new Category
        {
            Name = "Ice Slush Drinks",
            Description = "Refreshing ice slush beverages",
            TenantId = tenant.Id,
            IsActive = true,
        };
        iceSlushDrinks.SetTranslation("pt-BR", "Bebidas Geladas", "Bebidas refrescantes geladas");
        iceSlushDrinks.SetTranslation("es-AR", "Bebidas Heladas", "Bebidas refrescantes heladas");

        var roastPastry = new Category
        {
            Name = "Roast Pastry",
            Description = "Freshly baked roast pastries",
            TenantId = tenant.Id,
            IsActive = true,
        };
        roastPastry.SetTranslation("pt-BR", "Assados", "Assados frescos");
        roastPastry.SetTranslation("es-AR", "Asados", "Asados frescos");

        var categories = new[] { drinks, iceSlushDrinks, roastPastry };
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
            ("Sicilian Lemon", "Limão Siciliano", "Limón Siciliano", 9.50m),
            ("Açaí", "Açaí", "Açaí", 12.00m),
            ("Cocoa", "Cacau", "Cacao", 10.50m),
            ("Coffee", "Café", "Café", 10.00m),
            ("Detox", "Detox", "Detox", 11.50m)
        };

        // Categories and their preparation types
        var categoryTypes = new[]
        {
            (drinks, "Regular", "Normal", "Normal"),
            (iceSlushDrinks, "Ice Slush", "Gelada", "Helada")
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
                    "Ice Slush" => basePrice + 2.00m,
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
                        "Sicilian Lemon" => 110,
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
                            "Sicilian Lemon" => 48,
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

                // Map to actual image files in wwwroot/images
                var flavorFileName = flavorEn.ToLower().Replace(" ", "-") switch
                {
                    "passion-fruit" => "passion-fruit",
                    "sicilian-lemon" => "sicilian-lemon",
                    "coffee" => "coffe", // Note: file has single 'e'
                    _ => flavorEn.ToLower().Replace(" ", "-")
                };

                var imageFileName = type == "Regular"
                    ? $"sugarcane-{flavorFileName}.png"
                    : $"sugarcane-{flavorFileName}-ice-slush.png";

                var product = new Product
                {
                    Name = type == "Regular" ? $"{flavorEn}" : $"Ice Slush - {flavorEn}",
                    ProductCode = Guid.CreateVersion7().ToString().ToUpper(),
                    Description = $"Fresh {type.ToLower()} sugarcane juice with {flavorEn.ToLower()} flavor",
                    Brand = "Cana Brasil",
                    Unit = "500ml Cup",
                    Price = price,
                    TenantId = tenant.Id,
                    Category = category,
                    ProductImages = new List<ProductImage>
                    {
                        new ProductImage
                        {
                            Name = $"{flavorEn}",
                            FileName = imageFileName,
                            Url = $"/images/{imageFileName}",
                            AltText = $"{type} sugarcane juice with {flavorEn.ToLower()} flavor",
                            Size = 1024 * 200, // 200KB in bytes
                            Width = 400,
                            Height = 400,
                            MimeType = "image/png",
                            IsPrimary = true,
                            SortOrder = 0
                        }
                    }
                };

                // Add translations
                var translatedNamePtBr = type == "Regular" ? $"{flavorPtBr}" : $"Ice - {flavorPtBr}";
                var translatedNameEsAr = type == "Regular" ? $"Caña - {flavorEsAr}" : $"Caña Helado - {flavorEsAr}";

                product.SetTranslation("pt-BR",
                    translatedNamePtBr,
                    $"Caldo de cana fresco {typePtBr.ToLower()} com sabor {flavorPtBr.ToLower()}",
                    "Copo 500ml",
                    nutritionFacts);

                product.SetTranslation("es-AR",
                    translatedNameEsAr,
                    $"Jugo de caña fresco {typeEsAr.ToLower()} con sabor {flavorEsAr.ToLower()}",
                    "Vaso 500ml",
                    nutritionFacts);

                products.Add(product);
            }
        }

        // Add some roast pastries
        var pastryProducts = new[]
        {
            new Product
            {
                Name = "Roast Pastry - Chicken",
                ProductCode = Guid.CreateVersion7().ToString().ToUpper(),
                Description = "Crispy roast pastry filled with seasoned chicken",
                Brand = "Assados da Casa",
                Unit = "Piece",
                Price = 8.50m,
                TenantId = tenant.Id,
                Category = roastPastry,
                ProductImages = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Name = "Chicken Roast Pastry",
                        FileName = "roast-pasty-chicken.png",
                        Url = "/images/roast-pasty-chicken.png",
                        AltText = "Crispy roast pastry filled with seasoned chicken",
                        Size = 1024 * 180, // 180KB in bytes
                        Width = 400,
                        Height = 300,
                        MimeType = "image/png",
                        IsPrimary = true,
                        SortOrder = 0
                    }
                }
            },
            new Product
            {
                Name = "Roast Pastry - Meat",
                ProductCode = Guid.CreateVersion7().ToString().ToUpper(),
                Description = "Traditional roast pastry with chicken and seasoned ground beef",
                Brand = "Assados da Casa",
                Unit = "Piece",
                Price = 9.50m,
                TenantId = tenant.Id,
                Category = roastPastry,
                ProductImages = new List<ProductImage>
                {
                    new ProductImage
                    {
                        Name = "Meat Roast Pastry",
                        FileName = "roast-pasty-chicken-meat.png",
                        Url = "/images/roast-pasty-chicken-meat.png",
                        AltText = "Traditional roast pastry with chicken and seasoned ground beef",
                        Size = 1024 * 180, // 180KB in bytes
                        Width = 400,
                        Height = 300,
                        MimeType = "image/png",
                        IsPrimary = true,
                        SortOrder = 0
                    }
                }
            }
        };

        // Add translations to roast pastries
        pastryProducts[0].SetTranslation("pt-BR", "Assado de Frango", "Assado crocante recheado com frango temperado", "Unidade");
        pastryProducts[0].SetTranslation("es-AR", "Asado de Pollo", "Asado crujiente relleno de pollo condimentado", "Unidad");

        pastryProducts[1].SetTranslation("pt-BR", "Assado de Carne", "Assado tradicional carne moída temperada", "Unidade");
        pastryProducts[1].SetTranslation("es-AR", "Asado de Carne", "Asado tradicional carne picada condimentada", "Unidad");

        products.AddRange(pastryProducts);

        await _context.Products.AddRangeAsync(products);
        await _context.SaveChangesAsync();
    }

    private async Task SeedInventoryAsync()
    {
        if (await _context.InventoryLocations.AnyAsync() || await _context.RecipeComponents.AnyAsync())
        {
            return;
        }

        _logger.LogInformation("Seeding inventory locations, raw materials, and recipes...");

        var tenant = await _context.Tenants.FirstAsync();

        // Create default inventory location
        var defaultLocation = new InventoryLocation
        {
            Name = "Main Warehouse",
            Code = "MAIN",
            IsDefault = true,
            TenantId = tenant.Id
        };

        await _context.InventoryLocations.AddAsync(defaultLocation);
        await _context.SaveChangesAsync();

        // Create a category for raw materials
        var rawMaterialsCategory = new Category
        {
            Name = "Raw Materials",
            Description = "Ingredients and raw materials for production",
            TenantId = tenant.Id,
            IsActive = true,
            IsRawMaterial = true
        };
        rawMaterialsCategory.SetTranslation("pt-BR", "Matérias-Primas", "Ingredientes e matérias-primas para produção");
        rawMaterialsCategory.SetTranslation("es-AR", "Materias Primas", "Ingredientes y materias primas para producción");

        await _context.Categories.AddAsync(rawMaterialsCategory);
        await _context.SaveChangesAsync();

        // Create raw material products
        var sugarcaneProduct = new Product
        {
            Name = "Sugarcane-fruit",
            ProductCode = "RAW-SUGARCANE",
            Description = "Fresh sugarcane for juice extraction",
            Brand = "Farm Fresh",
            Unit = "kg",
            Price = 5.00m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        sugarcaneProduct.SetTranslation("pt-BR", "Cana-de-açúcar-fruta", "Cana-de-açúcar fresca para extração de suco", "kg");
        sugarcaneProduct.SetTranslation("es-AR", "Caña de azúcar-fruta", "Caña de azúcar fresca para extracción de jugo", "kg");

        var strawberryProduct = new Product
        {
            Name = "Strawberry-fruit",
            ProductCode = "RAW-STRAWBERRY",
            Description = "Fresh strawberries",
            Brand = "Farm Fresh",
            Unit = "g",
            Price = 0.03m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        strawberryProduct.SetTranslation("pt-BR", "Morango-fruta", "Morangos frescos", "g");
        strawberryProduct.SetTranslation("es-AR", "Fresa-fruta", "Fresas frescas", "g");

        var passionFruitProduct = new Product
        {
            Name = "Passion Fruit-fruit",
            ProductCode = "RAW-PASSIONFRUIT",
            Description = "Fresh passion fruit",
            Brand = "Farm Fresh",
            Unit = "g",
            Price = 0.025m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        passionFruitProduct.SetTranslation("pt-BR", "Maracujá-fruta", "Maracujá fresco", "g");
        passionFruitProduct.SetTranslation("es-AR", "Maracuyá-fruta", "Maracuyá fresco", "g");

        var lemonProduct = new Product
        {
            Name = "Lemon-fruit",
            ProductCode = "RAW-LEMON",
            Description = "Fresh lemon",
            Brand = "Farm Fresh",
            Unit = "g",
            Price = 0.02m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        lemonProduct.SetTranslation("pt-BR", "Limão-fruta", "Limão fresco", "g");
        lemonProduct.SetTranslation("es-AR", "Limón-fruta", "Limón fresco", "g");

        var sicilianLemonProduct = new Product
        {
            Name = "Sicilian Lemon-fruit",
            ProductCode = "RAW-SICILIANLEMON",
            Description = "Fresh Sicilian lemon",
            Brand = "Farm Fresh",
            Unit = "g",
            Price = 0.035m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        sicilianLemonProduct.SetTranslation("pt-BR", "Limão Siciliano-fruta", "Limão siciliano fresco", "g");
        sicilianLemonProduct.SetTranslation("es-AR", "Limón Siciliano-fruta", "Limón siciliano fresco", "g");

        var acaiProduct = new Product
        {
            Name = "Açaí-fruit",
            ProductCode = "RAW-ACAI",
            Description = "Fresh açaí pulp",
            Brand = "Farm Fresh",
            Unit = "g",
            Price = 0.08m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        acaiProduct.SetTranslation("pt-BR", "Açaí-fruta", "Polpa de açaí fresca", "g");
        acaiProduct.SetTranslation("es-AR", "Açaí-fruta", "Pulpa de açaí fresca", "g");

        var cocoaProduct = new Product
        {
            Name = "Cocoa-powder",
            ProductCode = "RAW-COCOA",
            Description = "Cocoa powder",
            Brand = "Farm Fresh",
            Unit = "g",
            Price = 0.05m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        cocoaProduct.SetTranslation("pt-BR", "Cacau-em-pó", "Cacau em pó", "g");
        cocoaProduct.SetTranslation("es-AR", "Cacao-en-polvo", "Cacao en polvo", "g");

        var coffeeProduct = new Product
        {
            Name = "Coffee-grain",
            ProductCode = "RAW-COFFEE",
            Description = "Ground coffee",
            Brand = "Farm Fresh",
            Unit = "g",
            Price = 0.04m,
            TenantId = tenant.Id,
            CategoryId = rawMaterialsCategory.Id
        };
        coffeeProduct.SetTranslation("pt-BR", "Café-grão", "Café moído", "g");
        coffeeProduct.SetTranslation("es-AR", "Café-grano", "Café molido", "g");

        var rawMaterials = new[]
        {
            sugarcaneProduct,
            strawberryProduct,
            passionFruitProduct,
            lemonProduct,
            sicilianLemonProduct,
            acaiProduct,
            cocoaProduct,
            coffeeProduct
        };

        await _context.Products.AddRangeAsync(rawMaterials);
        await _context.SaveChangesAsync();

        // Create BOM/Recipe components for finished products
        // Get finished products (the sugarcane juice products created earlier)
        var finishedProducts = await _context.Products
            .Where(p => p.TenantId == tenant.Id &&
                   (p.Name!.Contains("Pure") ||
                    p.Name.Contains("Strawberry") ||
                    p.Name.Contains("Passion Fruit") ||
                    p.Name.Contains("Lemon") ||
                    p.Name.Contains("Sicilian Lemon") ||
                    p.Name.Contains("Açaí") ||
                    p.Name.Contains("Cocoa") ||
                    p.Name.Contains("Coffee")))
            .ToListAsync();

        var recipeComponents = new List<RecipeComponent>();

        foreach (var product in finishedProducts)
        {
            // All products need sugarcane as base (except Ice Slush variations use less)
            var isIceSlush = product.Name!.Contains("Ice Slush");
            var baseSugarcaneAmount = isIceSlush ? 0.8m : 1.0m; // 800g for ice slush, 1kg for regular

            // Base sugarcane component for all products
            recipeComponents.Add(new RecipeComponent
            {
                ProductId = product.Id,
                ComponentProductId = sugarcaneProduct.Id,
                Quantity = baseSugarcaneAmount,
                Unit = UnitOfMeasure.Kilogram,
                TenantId = tenant.Id
            });

            // Add flavor-specific components
            if (product.Name.Contains("Strawberry"))
            {
                recipeComponents.Add(new RecipeComponent
                {
                    ProductId = product.Id,
                    ComponentProductId = strawberryProduct.Id,
                    Quantity = 100,
                    Unit = UnitOfMeasure.Gram,
                    TenantId = tenant.Id
                });
            }
            else if (product.Name.Contains("Passion Fruit"))
            {
                recipeComponents.Add(new RecipeComponent
                {
                    ProductId = product.Id,
                    ComponentProductId = passionFruitProduct.Id,
                    Quantity = 120,
                    Unit = UnitOfMeasure.Gram,
                    TenantId = tenant.Id
                });
            }
            else if (product.Name.Contains("Sicilian Lemon"))
            {
                recipeComponents.Add(new RecipeComponent
                {
                    ProductId = product.Id,
                    ComponentProductId = sicilianLemonProduct.Id,
                    Quantity = 80,
                    Unit = UnitOfMeasure.Gram,
                    TenantId = tenant.Id
                });
            }
            else if (product.Name.Contains("Lemon") && !product.Name.Contains("Sicilian"))
            {
                recipeComponents.Add(new RecipeComponent
                {
                    ProductId = product.Id,
                    ComponentProductId = lemonProduct.Id,
                    Quantity = 80,
                    Unit = UnitOfMeasure.Gram,
                    TenantId = tenant.Id
                });
            }
            else if (product.Name.Contains("Açaí"))
            {
                recipeComponents.Add(new RecipeComponent
                {
                    ProductId = product.Id,
                    ComponentProductId = acaiProduct.Id,
                    Quantity = 150,
                    Unit = UnitOfMeasure.Gram,
                    TenantId = tenant.Id
                });
            }
            else if (product.Name.Contains("Cocoa"))
            {
                recipeComponents.Add(new RecipeComponent
                {
                    ProductId = product.Id,
                    ComponentProductId = cocoaProduct.Id,
                    Quantity = 50,
                    Unit = UnitOfMeasure.Gram,
                    TenantId = tenant.Id
                });
            }
            else if (product.Name.Contains("Coffee"))
            {
                recipeComponents.Add(new RecipeComponent
                {
                    ProductId = product.Id,
                    ComponentProductId = coffeeProduct.Id,
                    Quantity = 30,
                    Unit = UnitOfMeasure.Gram,
                    TenantId = tenant.Id
                });
            }
            // Pure flavor only has sugarcane (already added above)
            // Detox would need its own ingredients (skipped for now)
        }

        await _context.RecipeComponents.AddRangeAsync(recipeComponents);
        await _context.SaveChangesAsync();

        // Initialize inventory items with some starting stock for raw materials
        var inventoryItems = new List<InventoryItem>
        {
            new InventoryItem
            {
                ProductId = sugarcaneProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 500, // 500 kg in stock
                SafetyStock = 50,
                ReorderPoint = 100
            },
            new InventoryItem
            {
                ProductId = strawberryProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 5000, // 5000 g in stock
                SafetyStock = 500,
                ReorderPoint = 1000
            },
            new InventoryItem
            {
                ProductId = passionFruitProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 6000, // 6000 g in stock
                SafetyStock = 600,
                ReorderPoint = 1200
            },
            new InventoryItem
            {
                ProductId = lemonProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 4000, // 4000 g in stock
                SafetyStock = 400,
                ReorderPoint = 800
            },
            new InventoryItem
            {
                ProductId = sicilianLemonProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 3000, // 3000 g in stock
                SafetyStock = 300,
                ReorderPoint = 600
            },
            new InventoryItem
            {
                ProductId = acaiProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 8000, // 8000 g in stock
                SafetyStock = 800,
                ReorderPoint = 1500
            },
            new InventoryItem
            {
                ProductId = cocoaProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 2500, // 2500 g in stock
                SafetyStock = 250,
                ReorderPoint = 500
            },
            new InventoryItem
            {
                ProductId = coffeeProduct.Id,
                InventoryLocationId = defaultLocation.Id,
                TenantId = tenant.Id,
                OnHand = 1500, // 1500 g in stock
                SafetyStock = 150,
                ReorderPoint = 300
            }
        };

        await _context.InventoryItems.AddRangeAsync(inventoryItems);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Inventory seeding completed successfully");
    }
}