using ArtStore.Application.Common.Interfaces;
using ArtStore.Domain.Entities;
using ArtStore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Tests.Fixtures;

/// <summary>
/// Test fixture for creating an in-memory database context for testing.
/// </summary>
public class ApplicationDbContextFixture : IDisposable
{
    public ApplicationDbContext Context { get; private set; }

    public ApplicationDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new ApplicationDbContext(options);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Seed Categories
        var category1 = new Category
        {
            Id = 1,
            Name = "Electronics",
            Description = "Electronic items",
            IsActive = true,
            TenantId = 1
        };

        var category2 = new Category
        {
            Id = 2,
            Name = "Clothing",
            Description = "Clothing items",
            IsActive = true,
            TenantId = 1
        };

        Context.Categories.AddRange(category1, category2);

        // Seed Products
        var product1 = new Product
        {
            Id = 1,
            Name = "Laptop",
            Description = "High performance laptop",
            Brand = "TechBrand",
            Unit = "piece",
            Price = 999.99m,
            ProductCode = "LAP001",
            IsActive = true,
            CategoryId = 1,
            TenantId = 1
        };

        var product2 = new Product
        {
            Id = 2,
            Name = "T-Shirt",
            Description = "Cotton T-Shirt",
            Brand = "FashionBrand",
            Unit = "piece",
            Price = 19.99m,
            ProductCode = "TSH001",
            IsActive = true,
            CategoryId = 2,
            TenantId = 1
        };

        Context.Products.AddRange(product1, product2);

        // Seed Coupons
        var coupon = new Coupon
        {
            Id = 1,
            Code = "SAVE10",
            Name = "Save 10%",
            Description = "10% off",
            Type = CouponType.Percentage,
            Value = 10,
            MinimumOrderAmount = 50,
            UsageLimit = 100,
            UsedCount = 5,
            IsActive = true,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(20),
            TenantId = 1
        };

        Context.Coupons.Add(coupon);

        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}
