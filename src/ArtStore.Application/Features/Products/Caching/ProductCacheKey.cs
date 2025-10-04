// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Extensions.Caching.Hybrid;

namespace ArtStore.Application.Features.Products.Caching;

public static class ProductCacheKey
{
    public const string GetAllCacheKey = "products:all";

    public static string GetProductByIdCacheKey(int id, string culture = "pt-BR")
    {
        return $"products:byid:{id}:{culture}";
    }

    public static string GetSearchCacheKey(string parameters)
    {
        return $"products:search:{parameters}";
    }

    public static string GetPaginationCacheKey(string parameters)
    {
        return $"products:pagination:{parameters}";
    }

    public static IEnumerable<string> Tags => new[] { "products" };

    public static async Task ClearAllAsync(HybridCache cache)
    {
        // Clear all culture-specific cache keys for GetAll
        var cultures = new[] { "pt-BR", "en-US", "es-AR" };

        var tasks = new List<ValueTask>();

        // Clear GetAll cache for all cultures
        foreach (var culture in cultures)
        {
            tasks.Add(cache.RemoveAsync($"{GetAllCacheKey}:{culture}"));
        }

        // Clear search and pagination cache - these would need a more sophisticated approach
        // For now, we'll rely on cache expiration for these

        await Task.WhenAll(tasks.Select(t => t.AsTask()));
    }

    public static async Task ClearProductAsync(HybridCache cache, int productId)
    {
        var cultures = new[] { "pt-BR", "en-US", "es-AR" };
        var tasks = new List<ValueTask>();

        // Clear specific product cache for all cultures
        foreach (var culture in cultures)
        {
            tasks.Add(cache.RemoveAsync(GetProductByIdCacheKey(productId, culture)));
        }

        // Also clear all GetAll caches since a product was modified
        foreach (var culture in cultures)
        {
            tasks.Add(cache.RemoveAsync($"{GetAllCacheKey}:{culture}"));
        }

        await Task.WhenAll(tasks.Select(t => t.AsTask()));
    }
}