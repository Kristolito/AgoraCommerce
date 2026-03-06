using AgoraCommerce.Api.Extensions;
using AgoraCommerce.Application.Features.Coupons;
using AgoraCommerce.Contracts.Common;
using AgoraCommerce.Contracts.Coupons;
using AgoraCommerce.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AgoraCommerce.Api.Controllers;

[ApiController]
[Route("api/v1/admin/coupons")]
public sealed class AdminCouponsController(ICouponService couponService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<CouponDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CouponDto>>> GetCoupons(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await couponService.AdminGetCouponsAsync(new AdminGetCouponsQuery(page, pageSize), cancellationToken);
        return Ok(result.ToPagedResponse(x => x.ToDto()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CouponDto>> GetCouponById(Guid id, CancellationToken cancellationToken)
    {
        var coupon = await couponService.AdminGetCouponByIdAsync(new AdminGetCouponByIdQuery(id), cancellationToken);
        return Ok(coupon.ToDto());
    }

    [HttpPost]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CouponDto>> CreateCoupon([FromBody] CreateCouponRequest request, CancellationToken cancellationToken)
    {
        var created = await couponService.CreateCouponAsync(
            new CreateCouponCommand(
                request.Code,
                (CouponType)request.Type,
                request.Amount,
                request.Currency,
                request.IsActive,
                request.ActiveFrom,
                request.ActiveTo,
                request.MaxRedemptions),
            cancellationToken);

        return CreatedAtAction(nameof(GetCouponById), new { id = created.Id }, created.ToDto());
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(CouponDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CouponDto>> UpdateCoupon(Guid id, [FromBody] UpdateCouponRequest request, CancellationToken cancellationToken)
    {
        var updated = await couponService.UpdateCouponAsync(
            new UpdateCouponCommand(
                id,
                (CouponType)request.Type,
                request.Amount,
                request.Currency,
                request.IsActive,
                request.ActiveFrom,
                request.ActiveTo,
                request.MaxRedemptions),
            cancellationToken);

        return Ok(updated.ToDto());
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeactivateCoupon(Guid id, CancellationToken cancellationToken)
    {
        await couponService.DeactivateCouponAsync(new DeactivateCouponCommand(id), cancellationToken);
        return NoContent();
    }
}
