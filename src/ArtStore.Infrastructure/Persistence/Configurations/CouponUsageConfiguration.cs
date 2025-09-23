using ArtStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArtStore.Infrastructure.Persistence.Configurations;

public class CouponUsageConfiguration : IEntityTypeConfiguration<CouponUsage>
{
    public void Configure(EntityTypeBuilder<CouponUsage> builder)
    {
        builder.HasKey(cu => cu.Id);

        builder.Property(cu => cu.CouponId)
            .IsRequired();

        builder.Property(cu => cu.OrderId)
            .IsRequired();

        builder.Property(cu => cu.UserId)
            .HasMaxLength(450);

        builder.Property(cu => cu.DiscountAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(cu => cu.UsedDate)
            .IsRequired();

        // Indexes for performance
        builder.HasIndex(cu => cu.CouponId);
        builder.HasIndex(cu => cu.OrderId);
        builder.HasIndex(cu => new { cu.CouponId, cu.UserId });

        // Relationships
        builder.HasOne(cu => cu.Coupon)
            .WithMany(c => c.CouponUsages)
            .HasForeignKey(cu => cu.CouponId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cu => cu.Order)
            .WithMany(o => o.CouponUsages)
            .HasForeignKey(cu => cu.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}