using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Coupon.Commands;

public class ValidateCouponCommand : ICommand<CouponValidationResult>
{
    public string Code { get; set; } = string.Empty;
    public decimal OrderTotal { get; set; }
    public string? UserId { get; set; }
}

public class CouponValidationResult
{
    public bool IsValid { get; set; }
    public string Message { get; set; } = string.Empty;
    public CouponDto? Coupon { get; set; }
    public decimal DiscountAmount { get; set; }
}