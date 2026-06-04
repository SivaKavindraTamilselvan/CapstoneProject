using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly IVendorCouponService _vendorCouponService;
    public VendorController(IVendorService vendorService,IVendorCouponService vendorCouponService)
    {
        _vendorService = vendorService;
        _vendorCouponService = vendorCouponService;
    }

    [Authorize(Policy = "VendorAdminOnly")]
    [HttpPost("RegisterVendorUser")]
    public async Task<ActionResult<ResponseRegisterVendorUserDTO>> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.RegisterVendorUser(requestRegisterVendorUserDTO, vendorUserId);
        return Ok(result);
    }

    [Authorize("VendorOnwerAndCouponVendorOnly")]
    [HttpPost("AddVendorCoupon")]
    public async Task<ActionResult<ResponseAddCouponDTO>> ResponseAddVendorCoupon(RequestAddCouponDTO requestAddCouponDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorCouponService.AddVendorCoupon(requestAddCouponDTO,vendorUserId);
        return Ok(result);
    }

    [Authorize("VendorOnwerAndCouponVendorOnly")]
    [HttpPost("AddProductCoupon")]
    public async Task<ActionResult<ResponseAddCouponDTO>> ResponseAddProductCoupon(RequestAddCouponProductDTO requestAddCouponProductDTO)
    {
        var result = await _vendorCouponService.AddProductCoupon(requestAddCouponProductDTO);
        return Ok(result);
    }
}