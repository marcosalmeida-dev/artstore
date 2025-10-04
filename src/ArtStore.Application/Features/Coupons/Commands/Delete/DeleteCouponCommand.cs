using ArtStore.Application.Common.Interfaces;
using ArtStore.Application.Common.Models;
using ArtStore.Shared.DTOs.Coupon.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Coupons.Commands.Delete;

public class DeleteCouponCommandHandler : ICommandHandler<DeleteCouponCommand, Result<int>>
{
    private readonly IApplicationDbContext _context;

    public DeleteCouponCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<int>> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var coupon = await _context.Coupons
                .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
            if (coupon == null)
            {
                return Result<int>.Failure("Coupon not found.");
            }

            // Check if coupon has been used
            var usageCount = await _context.CouponUsages
                .CountAsync(cu => cu.CouponId == request.Id, cancellationToken);
            if (usageCount > 0)
            {
                return Result<int>.Failure("Cannot delete a coupon that has been used.");
            }

            _context.Coupons.Remove(coupon);
            await _context.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(request.Id);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure($"Error deleting coupon: {ex.Message}");
        }
    }
}