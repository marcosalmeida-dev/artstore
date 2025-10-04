namespace ArtStore.Shared.DTOs.Coupon;

public class CouponDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public CouponType Type { get; set; }
    public decimal Value { get; set; }
    public decimal? MinimumOrderAmount { get; set; }
    public decimal? MaximumDiscountAmount { get; set; }
    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsOncePerCustomer { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }

    // Computed properties
    public bool IsExpired => DateTime.UtcNow > EndDate;
    public bool IsUsageLimitReached => UsageLimit.HasValue && UsedCount >= UsageLimit.Value;
    public bool IsValid => IsActive && !IsExpired && !IsUsageLimitReached && DateTime.UtcNow >= StartDate;
}

public enum CouponType
{
    Percentage = 1,
    FixedAmount = 2,
    FreeShipping = 3
}