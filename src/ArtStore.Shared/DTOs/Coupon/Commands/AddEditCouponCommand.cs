using System.ComponentModel.DataAnnotations;
using ArtStore.Shared.Interfaces.Command;

namespace ArtStore.Shared.DTOs.Coupon.Commands;

public class AddEditCouponCommand : ICommand<Result<int>>
{
    public int Id { get; set; }

    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public CouponType Type { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Value must be greater than 0")]
    public decimal Value { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Minimum order amount must be 0 or greater")]
    public decimal? MinimumOrderAmount { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Maximum discount amount must be 0 or greater")]
    public decimal? MaximumDiscountAmount { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Usage limit must be 1 or greater")]
    public int? UsageLimit { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsOncePerCustomer { get; set; } = false;
}