// src/ArtStore.Infrastructure/Persistence/Configurations/RecipeComponentConfiguration.cs
using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for RecipeComponent entity.
/// </summary>
public class RecipeComponentConfiguration : IEntityTypeConfiguration<RecipeComponent>
{
    public void Configure(EntityTypeBuilder<RecipeComponent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 3);

        builder.Property(x => x.Unit)
            .HasConversion<int>();

        // Unique index per (TenantId, ProductId, ComponentProductId)
        builder.HasIndex(x => new { x.TenantId, x.ProductId, x.ComponentProductId })
            .IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.ComponentProduct)
            .WithMany()
            .HasForeignKey(x => x.ComponentProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}