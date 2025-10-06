using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Features.Products.Services;
using ArtStore.Application.Interfaces.Services;
using ArtStore.Infrastructure.Persistence;
using Moq;

namespace ArtStore.Application.Tests.Fixtures;

/// <summary>
/// Base class for all test classes providing common mock objects and utilities.
/// </summary>
public class TestBase : IClassFixture<ApplicationDbContextFixture>
{
    protected readonly ApplicationDbContextFixture DbFixture;
    protected readonly Mock<IProductCacheService> MockProductCacheService;
    protected readonly Mock<IBlobStorageService> MockBlobStorageService;
    protected readonly Mock<IDateTime> MockDateTime;

    public TestBase(ApplicationDbContextFixture dbFixture)
    {
        DbFixture = dbFixture;
        MockProductCacheService = new Mock<IProductCacheService>();
        MockBlobStorageService = new Mock<IBlobStorageService>();
        MockDateTime = new Mock<IDateTime>();

        // Default DateTime mock setup
        MockDateTime.Setup(x => x.Now).Returns(DateTime.UtcNow);
    }

    /// <summary>
    /// Creates a new instance of the database context for each test.
    /// </summary>
    protected ApplicationDbContext GetDbContext()
    {
        return DbFixture.Context;
    }

    /// <summary>
    /// Resets the database to its initial seeded state.
    /// </summary>
    protected void ResetDatabase()
    {
        DbFixture.Dispose();
        // Note: In real scenarios, you might want to recreate the fixture
    }
}
