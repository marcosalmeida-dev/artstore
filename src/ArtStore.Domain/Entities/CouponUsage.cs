using ArtStore.Domain.Common.Entities;

namespace ArtStore.Domain.Entities;

public class CouponUsage : BaseTenantEntity<int>
{
    public int CouponId { get; set; }
    public long OrderId { get; set; }
    public string? UserId { get; set; }
    public decimal DiscountAmount { get; set; }
    public DateTime UsedDate { get; set; }

    // Navigation properties
    public virtual Coupon? Coupon { get; set; }
    public virtual Order? Order { get; set; }
}