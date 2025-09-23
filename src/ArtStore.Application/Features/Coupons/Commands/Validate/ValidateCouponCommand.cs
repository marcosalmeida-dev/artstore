using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Coupon;
using ArtStore.Shared.DTOs.Coupon.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Coupons.Commands.Validate;

public class ValidateCouponCommandHandler : ICommandHandler<ValidateCouponCommand, CouponValidationResult>
{
    private readonly IApplicationDbContext _context;

    public ValidateCouponCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CouponValidationResult> Handle(ValidateCouponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Code.ToLower() == request.Code.ToLower(), cancellationToken);
            if (coupon == null)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = "Coupon code not found."
                };
            }

            // Check if coupon is active
            if (!coupon.IsActive)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = "This coupon is not active."
                };
            }

            // Check date validity
            var now = DateTime.UtcNow;
            if (now < coupon.StartDate)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = "This coupon is not yet valid."
                };
            }

            if (now > coupon.EndDate)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = "This coupon has expired."
                };
            }

            // Check usage limit
            if (coupon.UsageLimit.HasValue && coupon.UsedCount >= coupon.UsageLimit.Value)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = "This coupon has reached its usage limit."
                };
            }

            // Check minimum order amount
            if (coupon.MinimumOrderAmount.HasValue && request.OrderTotal < coupon.MinimumOrderAmount.Value)
            {
                return new CouponValidationResult
                {
                    IsValid = false,
                    Message = $"Minimum order amount of {coupon.MinimumOrderAmount.Value:C} required for this coupon."
                };
            }

            // Check if user has already used this coupon (if once per customer)
            if (coupon.IsOncePerCustomer && !string.IsNullOrEmpty(request.UserId))
            {
                var hasUsed = await _context.CouponUsages
                    .AnyAsync(cu => cu.CouponId == coupon.Id && cu.UserId == request.UserId, cancellationToken);
                if (hasUsed)
                {
                    return new CouponValidationResult
                    {
                        IsValid = false,
                        Message = "You have already used this coupon."
                    };
                }
            }

            // Calculate discount amount
            var discountAmount = CalculateDiscountAmount(coupon, request.OrderTotal);

            var couponDto = new CouponDto
            {
                Id = coupon.Id,
                Code = coupon.Code,
                Name = coupon.Name,
                Description = coupon.Description,
                Type = (Shared.DTOs.Coupon.CouponType)coupon.Type,
                Value = coupon.Value,
                MinimumOrderAmount = coupon.MinimumOrderAmount,
                MaximumDiscountAmount = coupon.MaximumDiscountAmount,
                UsageLimit = coupon.UsageLimit,
                UsedCount = coupon.UsedCount,
                StartDate = coupon.StartDate,
                EndDate = coupon.EndDate,
                IsActive = coupon.IsActive,
                IsOncePerCustomer = coupon.IsOncePerCustomer,
                Created = coupon.Created ?? DateTime.MinValue,
                LastModified = coupon.LastModified
            };

            return new CouponValidationResult
            {
                IsValid = true,
                Message = "Coupon is valid.",
                Coupon = couponDto,
                DiscountAmount = discountAmount
            };
        }
        catch (Exception ex)
        {
            return new CouponValidationResult
            {
                IsValid = false,
                Message = $"Error validating coupon: {ex.Message}"
            };
        }
    }

    private decimal CalculateDiscountAmount(Domain.Entities.Coupon coupon, decimal orderTotal)
    {
        decimal discount = 0;

        switch (coupon.Type)
        {
            case Domain.Entities.CouponType.Percentage:
                discount = orderTotal * (coupon.Value / 100);
                break;
            case Domain.Entities.CouponType.FixedAmount:
                discount = coupon.Value;
                break;
            case Domain.Entities.CouponType.FreeShipping:
                // For free shipping, discount amount would depend on shipping cost
                // This could be calculated based on shipping rules
                discount = 0; // Placeholder
                break;
        }

        // Apply maximum discount limit if specified
        if (coupon.MaximumDiscountAmount.HasValue && discount > coupon.MaximumDiscountAmount.Value)
        {
            discount = coupon.MaximumDiscountAmount.Value;
        }

        // Ensure discount doesn't exceed order total
        if (discount > orderTotal)
        {
            discount = orderTotal;
        }

        return Math.Round(discount, 2);
    }
}