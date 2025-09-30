// src/ArtStore.Infrastructure/Persistence/Configurations/InventoryLocationConfiguration.cs
using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core configuration for InventoryLocation entity.
/// </summary>
public class InventoryLocationConfiguration : IEntityTypeConfiguration<InventoryLocation>
{
    public void Configure(EntityTypeBuilder<InventoryLocation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.Code)
            .HasMaxLength(32);

        // Unique index on (TenantId, Code) with filtered unique when Code is not null
        builder.HasIndex(x => new { x.TenantId, x.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}