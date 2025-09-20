using ArtStore.Application.Features.Products.Caching;
using Microsoft.Extensions.Caching.Hybrid;

namespace ArtStore.Application.Features.Products.Services;

public interface IProductCacheService
{
    Task InvalidateAllProductCacheAsync();
    Task InvalidateProductCacheAsync(int productId);
}

public class ProductCacheService : IProductCacheService
{
    private readonly HybridCache _cache;

    public ProductCacheService(HybridCache cache)
    {
        _cache = cache;
    }

    public async Task InvalidateAllProductCacheAsync()
    {
        await ProductCacheKey.ClearAllAsync(_cache);
    }

    public async Task InvalidateProductCacheAsync(int productId)
    {
        await ProductCacheKey.ClearProductAsync(_cache, productId);
    }
}