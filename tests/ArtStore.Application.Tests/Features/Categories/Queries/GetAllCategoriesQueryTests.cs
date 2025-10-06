using ArtStore.Application.Features.Categories.Queries.GetAll;
using ArtStore.Application.Tests.Fixtures;
using FluentAssertions;

namespace ArtStore.Application.Tests.Features.Categories.Queries;

public class GetAllCategoriesQueryTests : TestBase
{
    public GetAllCategoriesQueryTests(ApplicationDbContextFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task Handle_GetAllCategories_ShouldReturnAllCategories()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new GetAllCategoriesQueryHandler(context);
        var query = new GetAllCategoriesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCountGreaterThanOrEqualTo(2); // We seeded 2 categories
        result.Select(c => c!.Name).Should().Contain(new[] { "Electronics", "Clothing" });
    }

    [Fact]
    public async Task Handle_GetAllCategories_ShouldReturnActiveCategoriesOnly()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new GetAllCategoriesQueryHandler(context);
        var query = new GetAllCategoriesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(c => c!.IsActive);
    }

    [Fact]
    public async Task Handle_GetAllCategories_ShouldOrderCategoriesCorrectly()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new GetAllCategoriesQueryHandler(context);
        var query = new GetAllCategoriesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var categories = result.Where(c => c != null).Select(c => c!.Name).ToList();
        categories.Should().BeInAscendingOrder();
    }

    [Fact]
    public void GetAllCategoriesQuery_ShouldHaveCorrectCacheKey()
    {
        // Arrange & Act
        var query = new GetAllCategoriesQuery();

        // Assert
        query.CacheKey.Should().Contain("all-Categories");
    }
}
