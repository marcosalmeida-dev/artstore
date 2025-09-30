// src/ArtStore.Infrastructure/Persistence/Configurations/InventoryItemConfiguration.cs
using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for InventoryItem entity.
/// </summary>
public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.OnHand)
            .HasPrecision(18, 2);

        builder.Property(x => x.SafetyStock)
            .HasPrecision(18, 2);

        builder.Property(x => x.ReorderPoint)
            .HasPrecision(18, 2);

        // Unique index per (TenantId, ProductId, InventoryLocationId)
        builder.HasIndex(x => new { x.TenantId, x.ProductId, x.InventoryLocationId })
            .IsUnique();

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany(l => l.InventoryItems)
            .HasForeignKey(x => x.InventoryLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}