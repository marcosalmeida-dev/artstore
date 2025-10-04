using ArtStore.Application.Common.Models;
using ArtStore.Application.Features.Coupons.Commands.AddEdit;
using ArtStore.Application.Features.Coupons.Commands.Delete;
using ArtStore.Application.Features.Coupons.Queries.GetAll;
using ArtStore.Shared.DTOs.Coupon;
using ArtStore.Shared.DTOs.Coupon.Commands;
using ArtStore.Shared.Interfaces.Command;
using ArtStore.Shared.Interfaces.Query;
using Microsoft.AspNetCore.Mvc;

namespace ArtStore.UI.Controllers.Admin;

[Route("api/admin/[controller]")]
[ApiController]
public class CouponsController : ControllerBase
{
    private readonly IQueryHandler<GetAllCouponsQuery, IEnumerable<CouponDto?>> _getAllCouponsQueryHandler;
    private readonly IQueryHandler<GetCouponQuery, CouponDto?> _getCouponQueryHandler;
    private readonly ICommandHandler<AddEditCouponCommand, Result<int>> _addEditCouponCommandHandler;
    private readonly ICommandHandler<DeleteCouponCommand, Result<int>> _deleteCouponCommandHandler;

    public CouponsController(
        IQueryHandler<GetAllCouponsQuery, IEnumerable<CouponDto?>> getAllCouponsQueryHandler,
        IQueryHandler<GetCouponQuery, CouponDto?> getCouponQueryHandler,
        ICommandHandler<AddEditCouponCommand, Result<int>> addEditCouponCommandHandler,
        ICommandHandler<DeleteCouponCommand, Result<int>> deleteCouponCommandHandler)
    {
        _getAllCouponsQueryHandler = getAllCouponsQueryHandler;
        _getCouponQueryHandler = getCouponQueryHandler;
        _addEditCouponCommandHandler = addEditCouponCommandHandler;
        _deleteCouponCommandHandler = deleteCouponCommandHandler;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCoupons()
    {
        var coupons = await _getAllCouponsQueryHandler.Handle(new GetAllCouponsQuery());
        return Ok(coupons);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCoupon(int id)
    {
        var coupon = await _getCouponQueryHandler.Handle(new GetCouponQuery { Id = id });
        if (coupon == null)
        {
            return NotFound();
        }

        return Ok(coupon);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCoupon([FromBody] AddEditCouponCommand command)
    {
        command.Id = 0; // Ensure it's a create operation
        var result = await _addEditCouponCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCoupon(int id, [FromBody] AddEditCouponCommand command)
    {
        command.Id = id;
        var result = await _addEditCouponCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCoupon(int id)
    {
        var command = new DeleteCouponCommand { Id = id };
        var result = await _deleteCouponCommandHandler.Handle(command);

        if (result.Succeeded)
        {
            return Ok(result);
        }

        return BadRequest(result.Errors);
    }
}