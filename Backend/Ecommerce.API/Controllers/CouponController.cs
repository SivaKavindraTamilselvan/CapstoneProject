using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;
    private readonly IUserCouponService _userCouponService;
    public CouponController(ICouponService couponService, IUserCouponService userCouponService)
    {
        _couponService = couponService;
        _userCouponService = userCouponService;
    }
    [Authorize("CouponPolicy")]
    [HttpPost("AddCoupon")]
    public async Task<ActionResult<ResponseAddCouponDTO>> ResponseAddCoupon(RequestAddCouponDTO requestAddCouponDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int roleId = int.Parse(User.FindFirst(ClaimTypes.Role)!.Value);
        var result = await _couponService.AddCoupon(requestAddCouponDTO, roleId, UserId);
        return Ok(result);
    }

    [Authorize("CouponPolicy")]
    [HttpGet("coupons")]
    public async Task<ActionResult<ResponseAddCouponDTO>> GetAdminCoupons([FromQuery] AdminCouponFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _couponService.GetCouponsForAdmin(request, adminUserId);
        return Ok(result);
    }

    [Authorize("CouponPolicy")]
    [HttpGet("coupons/{couponId}")]
    public async Task<ActionResult<CouponDetailDto>> GetAdminCouponById([FromRoute] int couponId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _couponService.GetCouponByIdForAdmin(couponId, adminUserId);
        return Ok(result);
    }

    [Authorize("CouponPolicy")]
    [HttpPatch("coupons/deactivate/{couponId}")]
    public async Task<ActionResult<CouponDetailDto>> DeactivateCouponById([FromRoute] int couponId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _couponService.DeactivateCoupon(couponId, adminUserId);
        return Ok(result);
    }

    [Authorize("CouponPolicy")]
    [HttpPatch("coupons/activate/{couponId}")]
    public async Task<ActionResult<CouponDetailDto>> ActivateCouponById([FromRoute] int couponId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _couponService.ActivateCoupon(couponId, adminUserId);
        return Ok(result);
    }

    [Authorize("CouponPolicy")]
    [HttpPatch("coupons/update")]
    public async Task<ActionResult<CouponDetailDto>> UpdateCoupon(UpdateCouponDto update)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _couponService.UpdateCouponByIdForAdmin(update, adminUserId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("ActiveCoupons")]
    public async Task<ActionResult<ResponseGetAllCoupon>> GetAllCouponsByUser()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCouponService.GetAllActiveCoupons(UserId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("AvailableCoupons")]
    public async Task<ActionResult<ResponseGetAllCoupon>> GetAllAvailableCouponsByUser()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCouponService.GetAllAvailableCouponsUser(UserId);
        return Ok(result);
    }
}