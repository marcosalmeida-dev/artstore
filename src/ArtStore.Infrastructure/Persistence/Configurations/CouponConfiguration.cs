using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.Value)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.MinimumOrderAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.MaximumDiscountAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.StartDate)
            .IsRequired();

        builder.Property(c => c.EndDate)
            .IsRequired();

        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.IsOncePerCustomer)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.UsedCount)
            .IsRequired()
            .HasDefaultValue(0);

        // Index for fast code lookups
        builder.HasIndex(c => c.Code)
            .IsUnique();

        // Index for active coupons
        builder.HasIndex(c => new { c.IsActive, c.StartDate, c.EndDate });

        // Relationships
        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Coupon)
            .HasForeignKey(o => o.CouponId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(c => c.CouponUsages)
            .WithOne(cu => cu.Coupon)
            .HasForeignKey(cu => cu.CouponId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}