// src/ArtStore.Infrastructure/Persistence/Configurations/InventoryReservationConfiguration.cs
using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for InventoryReservation entity.
/// </summary>
public class InventoryReservationConfiguration : IEntityTypeConfiguration<InventoryReservation>
{
    public void Configure(EntityTypeBuilder<InventoryReservation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .HasPrecision(18, 2);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        // Index for querying reservations by order
        builder.HasIndex(x => new { x.TenantId, x.OrderId, x.OrderDetailId, x.Status });

        // Index for querying available inventory by product/location
        builder.HasIndex(x => new { x.TenantId, x.ProductId, x.InventoryLocationId, x.Status });

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