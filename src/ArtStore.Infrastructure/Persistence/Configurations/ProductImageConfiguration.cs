// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

#nullable disable
public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Url)
            .IsRequired()
            .HasMaxLength(2048);

        builder.Property(x => x.AltText)
            .HasMaxLength(500);

        builder.Property(x => x.Caption)
            .HasMaxLength(1000);

        builder.Property(x => x.MimeType)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("image/jpeg");

        builder.Property(x => x.Hash)
            .HasMaxLength(64); // SHA-256 hash length

        builder.Property(x => x.IsPrimary)
            .HasDefaultValue(false);

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0);

        // Foreign key relationship
        builder.HasOne(x => x.Product)
            .WithMany(x => x.Pictures)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_ProductImages_ProductId");

        builder.HasIndex(x => new { x.ProductId, x.IsPrimary })
            .HasDatabaseName("IX_ProductImages_ProductId_IsPrimary");

        builder.HasIndex(x => new { x.ProductId, x.SortOrder })
            .HasDatabaseName("IX_ProductImages_ProductId_SortOrder");

        builder.HasIndex(x => x.Hash)
            .HasDatabaseName("IX_ProductImages_Hash")
            .IsUnique(false); // Allow multiple images with same hash if needed

        // Table configuration
        builder.ToTable("ProductImages");

        // Ignore domain events from base entity
        builder.Ignore(e => e.DomainEvents);
    }
}