namespace ArtStore.Domain.Entities.Translations;

public abstract class BaseTranslation
{
    public string LanguageCode { get; set; } = string.Empty;
}

public class LanguageTranslation<T> : Dictionary<string, T> where T : BaseTranslation
{
    public T? GetTranslation(string languageCode)
    {
        return TryGetValue(languageCode, out var translation) ? translation : null;
    }

    public void SetTranslation(string languageCode, T translation)
    {
        translation.LanguageCode = languageCode;
        this[languageCode] = translation;
    }
}