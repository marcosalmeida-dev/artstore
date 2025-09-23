using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Domain.Entities;
using ArtStore.Shared.DTOs.Coupon.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Coupons.Commands.AddEdit;

public class AddEditCouponCommandHandler : ICommandHandler<AddEditCouponCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public AddEditCouponCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(AddEditCouponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if code is unique
            var query = _context.Coupons.Where(c => c.Code.ToLower() == request.Code.ToLower());
            if (request.Id != 0)
            {
                query = query.Where(c => c.Id != request.Id);
            }
            var codeExists = await query.AnyAsync(cancellationToken);
            if (codeExists)
            {
                return Result<int>.Failure("Coupon code already exists.");
            }

            // Validate date range
            if (request.EndDate <= request.StartDate)
            {
                return Result<int>.Failure("End date must be after start date.");
            }

            // Validate percentage value
            if (request.Type == Shared.DTOs.Coupon.CouponType.Percentage && request.Value > 100)
            {
                return Result<int>.Failure("Percentage value cannot exceed 100%.");
            }

            if (request.Id == 0)
            {
                // Create new coupon
                var coupon = new Coupon
                {
                    Code = request.Code.ToUpper(),
                    Name = request.Name,
                    Description = request.Description,
                    Type = (Domain.Entities.CouponType)request.Type,
                    Value = request.Value,
                    MinimumOrderAmount = request.MinimumOrderAmount,
                    MaximumDiscountAmount = request.MaximumDiscountAmount,
                    UsageLimit = request.UsageLimit,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    IsActive = request.IsActive,
                    IsOncePerCustomer = request.IsOncePerCustomer,
                    UsedCount = 0
                };

                _context.Coupons.Add(coupon);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(coupon.Id);
            }
            else
            {
                // Update existing coupon
                var existingCoupon = await _context.Coupons
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
                if (existingCoupon == null)
                {
                    return Result<int>.Failure("Coupon not found.");
                }

                existingCoupon.Code = request.Code.ToUpper();
                existingCoupon.Name = request.Name;
                existingCoupon.Description = request.Description;
                existingCoupon.Type = (Domain.Entities.CouponType)request.Type;
                existingCoupon.Value = request.Value;
                existingCoupon.MinimumOrderAmount = request.MinimumOrderAmount;
                existingCoupon.MaximumDiscountAmount = request.MaximumDiscountAmount;
                existingCoupon.UsageLimit = request.UsageLimit;
                existingCoupon.StartDate = request.StartDate;
                existingCoupon.EndDate = request.EndDate;
                existingCoupon.IsActive = request.IsActive;
                existingCoupon.IsOncePerCustomer = request.IsOncePerCustomer;

                _context.Coupons.Update(existingCoupon);
                await _context.SaveChangesAsync(cancellationToken);
                return Result<int>.Success(existingCoupon.Id);
            }
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error saving coupon: {ex.Message}");
        }
    }
}