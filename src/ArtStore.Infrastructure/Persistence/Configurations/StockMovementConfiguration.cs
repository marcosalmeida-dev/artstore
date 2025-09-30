// src/ArtStore.Infrastructure/Persistence/Configurations/StockMovementConfiguration.cs
using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for StockMovement entity.
/// </summary>
public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.Type)
            .HasConversion<int>();

        builder.Property(x => x.Reference)
            .HasMaxLength(128);

        builder.Property(x => x.Notes)
            .HasMaxLength(500);

        // Index for querying movements by product/location/time
        builder.HasIndex(x => new { x.TenantId, x.ProductId, x.InventoryLocationId, x.OccurredAt });

        // Index for querying movements by order
        builder.HasIndex(x => new { x.TenantId, x.OrderId });

        builder.HasOne(x => x.InventoryItem)
            .WithMany(i => i.StockMovements)
            .HasForeignKey(x => x.InventoryItemId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany()
            .HasForeignKey(x => x.InventoryLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}