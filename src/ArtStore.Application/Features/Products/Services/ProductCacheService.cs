using ArtStore.Application.Features.Products.Caching;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace ArtStore.Application.Features.Products.Services;

public interface IProductCacheService
{
    Task InvalidateAllProductCacheAsync();
    Task InvalidateProductCacheAsync(int productId);
}

public class ProductCacheService : IProductCacheService
{
    private readonly HybridCache _cache;
    private readonly ILogger<ProductCacheService> _logger;

    public ProductCacheService(HybridCache cache, ILogger<ProductCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task InvalidateAllProductCacheAsync()
    {
        try
        {
            await ProductCacheKey.ClearAllAsync(_cache);
            _logger.LogInformation("Successfully invalidated all product caches");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invalidate all product caches");
            throw;
        }
    }

    public async Task InvalidateProductCacheAsync(int productId)
    {
        try
        {
            await ProductCacheKey.ClearProductAsync(_cache, productId);
            _logger.LogInformation("Successfully invalidated cache for product {ProductId}", productId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to invalidate cache for product {ProductId}", productId);
            throw;
        }
    }
}