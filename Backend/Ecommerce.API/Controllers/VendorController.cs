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
    private readonly IVendorReturnService _vendorReturnService;
    public VendorController(IVendorService vendorService,IVendorReturnService vendorReturnService)
    {
        _vendorService = vendorService;
        _vendorReturnService = vendorReturnService;
    }

    [Authorize(Policy = "VendorAdminOnly")]
    [HttpPost("RegisterVendorUser")]
    public async Task<ActionResult<ResponseRegisterVendorUserDTO>> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.RegisterVendorUser(requestRegisterVendorUserDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPost("ReviewProductByVendor")]
    public async Task<ActionResult> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.ReviewProductByVendor(requestReviewOfProductDTO, vendorUserId);
        return Ok(result);
    }
}