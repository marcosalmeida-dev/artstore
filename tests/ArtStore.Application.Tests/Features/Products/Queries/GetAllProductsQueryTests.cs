using ArtStore.Application.Features.Products.Queries.GetAll;
using ArtStore.Application.Tests.Fixtures;
using FluentAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.DependencyInjection;

namespace ArtStore.Application.Tests.Features.Products.Queries;

public class GetAllProductsQueryTests : TestBase
{
    private readonly HybridCache _cache;

    public GetAllProductsQueryTests(ApplicationDbContextFixture dbFixture) : base(dbFixture)
    {
        var services = new ServiceCollection();
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(1)
            };
        });
        var provider = services.BuildServiceProvider();
        _cache = provider.GetRequiredService<HybridCache>();
    }

    [Fact]
    public async Task Handle_GetAllProducts_ShouldReturnAllActiveProducts()
    {
        var context = GetDbContext();
        var handler = new GetAllProductsQueryHandler(context, _cache);
        var query = new GetAllProductsQuery { Culture = "en-US" };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Should().OnlyContain(p => p != null && p.IsActive);
    }

    [Fact]
    public async Task Handle_GetProductById_ShouldReturnCorrectProduct()
    {
        var context = GetDbContext();
        var handler = new GetAllProductsQueryHandler(context, _cache);
        var query = new GetProductQuery { Id = 1, Culture = "en-US" };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Laptop");
        result.Brand.Should().Be("TechBrand");
        result.Price.Should().Be(999.99m);
    }

    [Fact]
    public async Task Handle_GetProductById_WhenProductNotFound_ShouldReturnNull()
    {
        var context = GetDbContext();
        var handler = new GetAllProductsQueryHandler(context, _cache);
        var query = new GetProductQuery { Id = 9999, Culture = "en-US" };

        var result = await handler.Handle(query, CancellationToken.None);

        result.Should().BeNull();
    }

    [Fact]
    public void GetAllProductsQuery_ShouldHaveCorrectCacheKey()
    {
        var query = new GetAllProductsQuery { Culture = "pt-BR" };
        var cacheKey = query.CacheKey;
        cacheKey.Should().Contain("all-Products");
        cacheKey.Should().Contain("pt-BR");
    }

    [Fact]
    public void GetProductQuery_ShouldHaveCorrectCacheKey()
    {
        var query = new GetProductQuery { Id = 123, Culture = "en-US" };
        var cacheKey = query.CacheKey;
        cacheKey.Should().Contain("123");
        cacheKey.Should().Contain("en-US");
    }
}
