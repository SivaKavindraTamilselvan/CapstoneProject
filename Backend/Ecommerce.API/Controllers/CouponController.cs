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
    private readonly IVendorCouponService _vendorCouponService;
    private readonly IUserCouponService _userCouponService;
    public CouponController(IVendorCouponService vendorCouponService,IUserCouponService userCouponService)
    {
        _vendorCouponService = vendorCouponService;
        _userCouponService = userCouponService;
    }
    [Authorize("CouponPolicy")]
    [HttpPost("AddCoupon")]
    public async Task<ActionResult<ResponseAddCouponDTO>> ResponseAddCoupon(RequestAddCouponDTO requestAddCouponDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        int roleId = int.Parse(User.FindFirst(ClaimTypes.Role)!.Value);
        var result = await _vendorCouponService.AddCoupon(requestAddCouponDTO, roleId, UserId);
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
        var result = await _userCouponService.GetAllAvailableCoupons(UserId);
        return Ok(result);
    }
}