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
    public CouponController(IVendorCouponService vendorCouponService)
    {
        _vendorCouponService = vendorCouponService;
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

    [Authorize("VendorOnwerAndCouponVendorOnly")]
    [HttpPost("AddProductCoupon")]
    public async Task<ActionResult<ResponseAddCouponDTO>> ResponseAddProductCoupon(RequestAddCouponProductDTO requestAddCouponProductDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorCouponService.AddCouponProduct(requestAddCouponProductDTO,vendorUserId);
        return Ok(result);
    }
}