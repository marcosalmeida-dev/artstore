# Translation System Usage Examples

This document demonstrates how to use the JSON-based translation system for Categories and Products.

## Basic Usage

### Setting Translations

```csharp
using ArtStore.Domain.Extensions;

// For Categories
var category = new Category 
{ 
    Name = "Electronics", // Default language
    Description = "Electronic products" 
};

// Add translations
category.SetTranslation("es", "Electrónicos", "Productos electrónicos");
category.SetTranslation("fr", "Électronique", "Produits électroniques");

// For Products
var product = new Product 
{ 
    Name = "Laptop", // Default language
    Description = "Gaming laptop",
    Unit = "piece"
};

// Add translations with nutrition facts
var nutritionFacts = new NutritionFacts
{
    Calories = 0, // Not applicable for electronics
    AdditionalNutrients = new Dictionary<string, decimal>
    {
        ["Warranty"] = 24 // 24 months
    }
};

product.SetTranslation("es", "Portátil", "Portátil para juegos", "pieza", nutritionFacts);
product.SetTranslation("fr", "Ordinateur portable", "Ordinateur portable de jeu", "pièce", nutritionFacts);
```

### Retrieving Translations

```csharp
// Get localized category name
var localizedName = category.GetLocalizedName("es"); // Returns "Electrónicos"
var fallbackName = category.GetLocalizedName("de"); // Falls back to English, returns "Electronics"

// Get localized product information
var localizedProductName = product.GetLocalizedName("fr"); // Returns "Ordinateur portable"
var localizedUnit = product.GetLocalizedUnit("es"); // Returns "pieza"
var localizedNutritionFacts = product.GetLocalizedNutritionFacts("es");
```

### Working with Nutrition Facts

```csharp
// For food products
var foodProduct = new Product 
{ 
    Name = "Organic Apple",
    Description = "Fresh organic apple",
    Unit = "kg"
};

var nutritionFacts = new NutritionFacts
{
    Calories = 52,
    Carbohydrates = 14,
    Fiber = 2.4m,
    Sugar = 10,
    Protein = 0.3m,
    Fat = 0.2m,
    AdditionalNutrients = new Dictionary<string, decimal>
    {
        ["Vitamin C"] = 4.6m,
        ["Potassium"] = 107
    }
};

foodProduct.SetTranslation("es", "Manzana Orgánica", "Manzana orgánica fresca", "kg", nutritionFacts);
```

## Database Storage

The translations are stored as JSON columns in the database:

```sql
-- Category translations example
{
  "es": {
    "LanguageCode": "es",
    "Name": "Electrónicos",
    "Description": "Productos electrónicos"
  },
  "fr": {
    "LanguageCode": "fr", 
    "Name": "Électronique",
    "Description": "Produits électroniques"
  }
}

-- Product translations example
{
  "es": {
    "LanguageCode": "es",
    "Name": "Manzana Orgánica",
    "Description": "Manzana orgánica fresca",
    "Unit": "kg",
    "NutritionFacts": {
      "Calories": 52,
      "Carbohydrates": 14,
      "Fiber": 2.4,
      "Sugar": 10,
      "Protein": 0.3,
      "Fat": 0.2,
      "AdditionalNutrients": {
        "Vitamin C": 4.6,
        "Potassium": 107
      }
    }
  }
}
```

## Entity Framework Usage

```csharp
// Query with translations
var categories = await context.Categories
    .Where(c => c.TenantId == tenantId)
    .ToListAsync();

foreach (var category in categories)
{
    var localizedName = category.GetLocalizedName(userLanguage);
    Console.WriteLine($"Category: {localizedName}");
}
```

## API Usage Example

```csharp
// In your controller or service
public async Task<CategoryDto> GetCategoryAsync(int id, string languageCode = "en")
{
    var category = await _context.Categories.FindAsync(id);
    
    return new CategoryDto
    {
        Id = category.Id,
        Name = category.GetLocalizedName(languageCode),
        Description = category.GetLocalizedDescription(languageCode)
    };
}
```

## Migration Considerations

When you create a migration for these changes, EF Core will:
1. Add a `Translations` column to both `Categories` and `Products` tables
2. Configure them as JSON columns (nvarchar(max) on SQL Server)
3. Allow null values since translations are optional

The system gracefully falls back to the default properties when translations are not available.