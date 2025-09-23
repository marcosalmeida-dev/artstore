using ArtStore.Domain.Common.Entities;

namespace ArtStore.Domain.Entities;

public class Coupon : BaseTenantEntity<int>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CouponType Type { get; set; }
    public decimal Value { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public new bool IsActive { get; set; } = true;
    public bool IsOncePerCustomer { get; set; } = false;

    // Navigation properties
    public virtual ICollection<Order>? Orders { get; set; }
    public virtual ICollection<CouponUsage>? CouponUsages { get; set; }
}

public enum CouponType
{
    Percentage = 1,
    FixedAmount = 2,
    FreeShipping = 3
}