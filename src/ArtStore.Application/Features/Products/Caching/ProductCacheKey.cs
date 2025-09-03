﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ArtStore.Application.Common.FusionCache;

namespace ArtStore.Application.Features.Products.Caching;

public static class ProductCacheKey
{
    public const string GetAllCacheKey = "all-Products";
    public static string GetProductByIdCacheKey(int id)
    {
        return $"GetProductById,{id}";
    }
    public static string GetSearchCacheKey(string parameters)
    {
        return $"SearchProductsQuery,{parameters}";
    }
    public static IEnumerable<string>? Tags => new string[] { "product" };
    public static void Refresh()
    {
        FusionCacheFactory.RemoveByTags(Tags);
    }

}
