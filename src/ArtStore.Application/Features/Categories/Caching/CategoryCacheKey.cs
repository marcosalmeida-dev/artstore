namespace ArtStore.Application.Features.Categories.Caching;

public static class CategoryCacheKey
{
    public const string GetAllCacheKey = "all-Categories";

    public static string GetCategoryByIdCacheKey(int id)
    {
        return $"GetCategoryById,{id}";
    }

    public static string GetSearchCacheKey(string parameters)
    {
        return $"SearchCategoriesQuery,{parameters}";
    }

    public static IEnumerable<string>? Tags => new string[] { "category" };
}