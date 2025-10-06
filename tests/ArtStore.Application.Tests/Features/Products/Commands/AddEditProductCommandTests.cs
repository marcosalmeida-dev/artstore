using ArtStore.Application.Features.Products.Commands.AddEdit;
using ArtStore.Application.Tests.Fixtures;
using ArtStore.Domain.Entities;
using ArtStore.Shared.DTOs.Product.Commands;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Tests.Features.Products.Commands;

public class AddEditProductCommandTests : TestBase
{
    public AddEditProductCommandTests(ApplicationDbContextFixture dbFixture) : base(dbFixture)
    {
    }

    [Fact]
    public async Task Handle_WhenCreatingNewProduct_ShouldAddProductToDatabase()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new AddEditProductCommandHandler(
            context,
            MockProductCacheService.Object,
            MockBlobStorageService.Object
        );

        var command = new AddEditProductCommand
        {
            Id = 0, // New product
            Name = "New Product",
            Description = "Test Description",
            Brand = "TestBrand",
            Unit = "piece",
            Price = 49.99m,
            ProductCode = "TEST001",
            IsActive = true,
            CategoryId = 1
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);

        var product = await context.Products.FindAsync(result.Data);
        product.Should().NotBeNull();
        product!.Name.Should().Be("New Product");
        product.Price.Should().Be(49.99m);
        product.Brand.Should().Be("TestBrand");
    }

    [Fact]
    public async Task Handle_WhenUpdatingExistingProduct_ShouldUpdateProductInDatabase()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new AddEditProductCommandHandler(
            context,
            MockProductCacheService.Object,
            MockBlobStorageService.Object
        );

        var command = new AddEditProductCommand
        {
            Id = 1, // Existing product from seed
            Name = "Updated Laptop",
            Description = "Updated description",
            Brand = "UpdatedBrand",
            Unit = "piece",
            Price = 1299.99m,
            ProductCode = "LAP001",
            IsActive = true,
            CategoryId = 1
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeTrue();

        var product = await context.Products.FindAsync(1);
        product.Should().NotBeNull();
        product!.Name.Should().Be("Updated Laptop");
        product.Price.Should().Be(1299.99m);
        product.Brand.Should().Be("UpdatedBrand");
    }

    [Fact]
    public async Task Handle_WhenProductIdDoesNotExist_ShouldReturnFailure()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new AddEditProductCommandHandler(
            context,
            MockProductCacheService.Object,
            MockBlobStorageService.Object
        );

        var command = new AddEditProductCommand
        {
            Id = 9999, // Non-existent product
            Name = "Non-existent Product",
            Description = "Test",
            Brand = "Test",
            Unit = "piece",
            Price = 10m,
            ProductCode = "TEST",
            IsActive = true,
            CategoryId = 1
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Succeeded.Should().BeFalse();
        result.Errors.Should().Contain(msg => msg.Contains("not found"));
    }

    [Fact]
    public async Task Handle_WhenCreatingProduct_ShouldInvalidateCache()
    {
        // Arrange
        var context = GetDbContext();
        var handler = new AddEditProductCommandHandler(
            context,
            MockProductCacheService.Object,
            MockBlobStorageService.Object
        );

        var command = new AddEditProductCommand
        {
            Id = 0,
            Name = "Cache Test Product",
            Description = "Test",
            Brand = "Test",
            Unit = "piece",
            Price = 25m,
            ProductCode = "CACHE001",
            IsActive = true,
            CategoryId = 1
        };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        MockProductCacheService.Verify(x => x.InvalidateAllProductCacheAsync(), Moq.Times.Once);
    }
}
