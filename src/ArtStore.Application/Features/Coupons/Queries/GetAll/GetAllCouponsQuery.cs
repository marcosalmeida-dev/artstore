using ArtStore.Application.Common.Interfaces;
using ArtStore.Shared.DTOs.Coupon;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.EntityFrameworkCore;

namespace ArtStore.Application.Features.Coupons.Queries.GetAll;

public class GetAllCouponsQuery : IQuery<IEnumerable<CouponDto>>
{
}

public class GetCouponQuery : IQuery<CouponDto>
{
    public required int Id { get; set; }
}

public class GetAllCouponsQueryHandler :
    IQueryHandler<GetAllCouponsQuery, IEnumerable<CouponDto?>>,
    IQueryHandler<GetCouponQuery, CouponDto?>
{
    private readonly IApplicationDbContext _context;

    public GetAllCouponsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<CouponDto?>> Handle(GetAllCouponsQuery request, CancellationToken cancellationToken)
    {
        var coupons = await _context.Coupons
            .OrderByDescending(c => c.Created)
            .ToListAsync(cancellationToken);

        return coupons.Select(c => new CouponDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            Description = c.Description,
            Type = (Shared.DTOs.Coupon.CouponType)c.Type,
            Value = c.Value,
            MinimumOrderAmount = c.MinimumOrderAmount,
            MaximumDiscountAmount = c.MaximumDiscountAmount,
            UsageLimit = c.UsageLimit,
            UsedCount = c.UsedCount,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            IsActive = c.IsActive,
            IsOncePerCustomer = c.IsOncePerCustomer,
            Created = c.Created ?? DateTime.MinValue,
            LastModified = c.LastModified
        }).ToList();
    }

    public async Task<CouponDto?> Handle(GetCouponQuery request, CancellationToken cancellationToken)
    {
        var coupon = await _context.Coupons
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
        if (coupon == null)
        {
            return null;
        }

        return new CouponDto
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
    }
}