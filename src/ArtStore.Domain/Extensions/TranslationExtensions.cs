using ArtStore.Domain.Entities;
using ArtStore.Domain.Entities.Translations;

namespace ArtStore.Domain.Extensions;

public static class TranslationExtensions
{
    public static string GetLocalizedName(this Category category, string languageCode, string? fallbackLanguageCode = "en")
    {
        var translation = category.Translations?.GetTranslation(languageCode);
        if (!string.IsNullOrEmpty(translation?.Name))
        {
            return translation.Name;
        }

        if (!string.IsNullOrEmpty(fallbackLanguageCode))
        {
            translation = category.Translations?.GetTranslation(fallbackLanguageCode);
            if (!string.IsNullOrEmpty(translation?.Name))
            {
                return translation.Name;
            }
        }

        return category.Name;
    }

    public static string? GetLocalizedDescription(this Category category, string languageCode, string? fallbackLanguageCode = "en")
    {
        var translation = category.Translations?.GetTranslation(languageCode);
        if (!string.IsNullOrEmpty(translation?.Description))
        {
            return translation.Description;
        }

        if (!string.IsNullOrEmpty(fallbackLanguageCode))
        {
            translation = category.Translations?.GetTranslation(fallbackLanguageCode);
            if (!string.IsNullOrEmpty(translation?.Description))
            {
                return translation.Description;
            }
        }

        return category.Description;
    }

    public static string? GetLocalizedName(this Product product, string languageCode, string? fallbackLanguageCode = "en-US")
    {
        var translation = product.Translations?.GetTranslation(languageCode);
        if (!string.IsNullOrEmpty(translation?.Name))
        {
            return translation.Name;
        }

        if (!string.IsNullOrEmpty(fallbackLanguageCode))
        {
            translation = product.Translations?.GetTranslation(fallbackLanguageCode);
            if (!string.IsNullOrEmpty(translation?.Name))
            {
                return translation.Name;
            }
        }

        return product.Name;
    }

    public static string? GetLocalizedDescription(this Product product, string languageCode, string? fallbackLanguageCode = "en")
    {
        var translation = product.Translations?.GetTranslation(languageCode);
        if (!string.IsNullOrEmpty(translation?.Description))
        {
            return translation.Description;
        }

        if (!string.IsNullOrEmpty(fallbackLanguageCode))
        {
            translation = product.Translations?.GetTranslation(fallbackLanguageCode);
            if (!string.IsNullOrEmpty(translation?.Description))
            {
                return translation.Description;
            }
        }

        return product.Description;
    }

    public static string? GetLocalizedUnit(this Product product, string languageCode, string? fallbackLanguageCode = "en")
    {
        var translation = product.Translations?.GetTranslation(languageCode);
        if (!string.IsNullOrEmpty(translation?.Unit))
        {
            return translation.Unit;
        }

        if (!string.IsNullOrEmpty(fallbackLanguageCode))
        {
            translation = product.Translations?.GetTranslation(fallbackLanguageCode);
            if (!string.IsNullOrEmpty(translation?.Unit))
            {
                return translation.Unit;
            }
        }

        return product.Unit;
    }

    public static NutritionFacts? GetLocalizedNutritionFacts(this Product product, string languageCode, string? fallbackLanguageCode = "en")
    {
        var translation = product.Translations?.GetTranslation(languageCode);
        if (translation?.NutritionFacts != null)
        {
            return translation.NutritionFacts;
        }

        if (!string.IsNullOrEmpty(fallbackLanguageCode))
        {
            translation = product.Translations?.GetTranslation(fallbackLanguageCode);
            if (translation?.NutritionFacts != null)
            {
                return translation.NutritionFacts;
            }
        }

        return null;
    }

    public static void SetTranslation(this Category category, string languageCode, string name, string? description = null)
    {
        category.Translations ??= new CategoryTranslationsJson();
        category.Translations.SetTranslation(languageCode, new CategoryTranslation
        {
            Name = name,
            Description = description
        });
    }

    public static void SetTranslation(this Product product, string languageCode, string? name = null, string? description = null, string? unit = null, NutritionFacts? nutritionFacts = null)
    {
        product.Translations ??= new ProductTranslationsJson();
        product.Translations.SetTranslation(languageCode, new ProductTranslation
        {
            Name = name,
            Description = description,
            Unit = unit,
            NutritionFacts = nutritionFacts
        });
    }
}