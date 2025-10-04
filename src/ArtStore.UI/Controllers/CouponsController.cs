using ArtStore.Application.Features.Coupons.Commands.Validate;
using ArtStore.Shared.DTOs.Coupon.Commands;
using ArtStore.Shared.Interfaces.Command;
using Microsoft.AspNetCore.Mvc;

namespace ArtStore.UI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponsController : ControllerBase
{
    private readonly ICommandHandler<ValidateCouponCommand, CouponValidationResult> _validateCouponCommandHandler;

    public CouponsController(ICommandHandler<ValidateCouponCommand, CouponValidationResult> validateCouponCommandHandler)
    {
        _validateCouponCommandHandler = validateCouponCommandHandler;
    }

    [HttpPost("validate")]
    public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponCommand command)
    {
        var result = await _validateCouponCommandHandler.Handle(command);
        return Ok(result);
    }
}