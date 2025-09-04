namespace ArtStore.Domain.Entities.Translations;

public class NutritionFacts
{
    public decimal? Calories { get; set; }
    public decimal? Fat { get; set; }
    public decimal? SaturatedFat { get; set; }
    public decimal? Carbohydrates { get; set; }
    public decimal? Fiber { get; set; }
    public decimal? Sugar { get; set; }
    public decimal? Protein { get; set; }
    public decimal? Sodium { get; set; }
    public Dictionary<string, decimal>? AdditionalNutrients { get; set; }
}

public class ProductTranslation : BaseTranslation
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Unit { get; set; }
    public NutritionFacts? NutritionFacts { get; set; }
}

public class ProductTranslationsJson : LanguageTranslation<ProductTranslation>
{
}