using System.Text.Json;
using ArtStore.Domain.Entities.Translations;
using ArtStore.Shared.Interfaces.Serialization;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.HasOne(x => x.Tenant)
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentCategory)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.TenantId, x.Name });

        // Configure JSON column for translations
        builder.Property(e => e.Translations)
            .HasConversion(
                v => JsonSerializer.Serialize(v, DefaultJsonSerializerOptions.Options),
                v => JsonSerializer.Deserialize<CategoryTranslationsJson>(v, DefaultJsonSerializerOptions.Options),
                new ValueComparer<CategoryTranslationsJson>(
                    (c1, c2) => JsonSerializer.Serialize(c1, DefaultJsonSerializerOptions.Options) == JsonSerializer.Serialize(c2, DefaultJsonSerializerOptions.Options),
                    c => c == null ? 0 : JsonSerializer.Serialize(c, DefaultJsonSerializerOptions.Options).GetHashCode(),
                    c => JsonSerializer.Deserialize<CategoryTranslationsJson>(JsonSerializer.Serialize(c, DefaultJsonSerializerOptions.Options), DefaultJsonSerializerOptions.Options)));
    }
}