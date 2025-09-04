namespace ArtStore.Domain.Entities.Translations;

public class CategoryTranslation : BaseTranslation
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class CategoryTranslationsJson : LanguageTranslation<CategoryTranslation>
{
}