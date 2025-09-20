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
        await cache.RemoveByTagAsync("products");
    }
    
    public static async Task ClearProductAsync(HybridCache cache, int productId)
    {
        await cache.RemoveAsync(GetProductByIdCacheKey(productId, "pt-BR"));
        await cache.RemoveAsync(GetProductByIdCacheKey(productId, "en-US"));
        await cache.RemoveAsync(GetProductByIdCacheKey(productId, "es-AR"));
        await cache.RemoveByTagAsync("products");
    }
}