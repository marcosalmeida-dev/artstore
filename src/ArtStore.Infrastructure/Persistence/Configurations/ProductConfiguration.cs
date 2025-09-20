// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.Json;
using ArtStore.Domain.Entities.Translations;
using ArtStore.Shared.Interfaces.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;
#nullable disable
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(80).IsRequired(false);
        
        builder.HasIndex(x => x.ProductCode).IsUnique();
        builder.Property(x => x.ProductCode).HasMaxLength(50).IsRequired();
        
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Brand).HasMaxLength(100);
        builder.Property(x => x.Unit).HasMaxLength(50);

        // Configure relationship with Category
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure relationship with ProductImages
        builder.HasMany(x => x.Pictures)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.DomainEvents);

        // Configure JSON column for translations
        builder.Property(e => e.Translations)
            .HasConversion(
                v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
                v => JsonSerializer.Deserialize<ProductTranslationsJson>(v, DefaultJsonSerializerOptions.Options),
                new ValueComparer<ProductTranslationsJson>(
                    (c1, c2) => JsonSerializer.Serialize(c1, DefaultJsonSerializerOptions.Options) == JsonSerializer.Serialize(c2, DefaultJsonSerializerOptions.Options),
                    c => c == null ? 0 : JsonSerializer.Serialize(c, DefaultJsonSerializerOptions.Options).GetHashCode(),
                    c => JsonSerializer.Deserialize<ProductTranslationsJson>(JsonSerializer.Serialize(c, DefaultJsonSerializerOptions.Options), DefaultJsonSerializerOptions.Options)));
    }
}