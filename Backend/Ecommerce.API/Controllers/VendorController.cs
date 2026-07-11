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
    public VendorController(IVendorService vendorService, IVendorReturnService vendorReturnService)
    {
        _vendorService = vendorService;
        _vendorReturnService = vendorReturnService;
    }

    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPost("RegisterVendorUser")]
    public async Task<ActionResult<ResponseRegisterVendorUserDTO>> RegisterVendorUser(RequestRegisterVendorUserDTO requestRegisterVendorUserDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.RegisterVendorUser(requestRegisterVendorUserDTO, vendorUserId);
        return Ok(result);
    }

    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpGet("vendor-user")]
    public async Task<ActionResult<ResponseRegisterVendorUserDTO>> GetAllVendorUser([FromQuery] RequestVendorUserFilter request)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.GetAllVendorUser(request, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpGet("vendor-user/{vendorUserId}")]
    public async Task<ActionResult<ResponseRegisterVendorUserDTO>> GetVendorUserByVendorUserId([FromRoute] int vendorUserId)
    {
        int loggedUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.GetVendorUserByUserId(vendorUserId, loggedUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("vendor-users/{adminUserId}/deactivate")]
    public async Task<IActionResult> DeactivateAdminUser(int adminUserId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.DeactivateAdminUser(adminUserId, UserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("vendor-users/{adminUserId}/activate")]
    public async Task<IActionResult> ActivateAdminUser(int adminUserId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.ActivateAdminUser(adminUserId, UserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("ReviewProductByVendor")]
    public async Task<ActionResult> ReviewProduct(RequestReviewOfProductDTO requestReviewOfProductDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.ReviewProductByVendor(requestReviewOfProductDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("ReviewProductVariantByVendor")]
    public async Task<ActionResult> ReviewProductVariant(RequestReviewOfProductVariantDTO requestReviewOfProductDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorService.ReviewProductVariant(requestReviewOfProductDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPut("ReviewReturnProductByVendor")]
    public async Task<ActionResult<ResponseReviewReturnDTO>> ReviewReturnProduct(RequestReviewReturnDTO requestReviewReturnDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorReturnService.ReviewReturnOrder(requestReviewReturnDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPost("ReviewReturnOriginalProductByVendor")]
    public async Task<ActionResult<ResponseReviewReturnDTO>> ReviewReturnOriginalProduct(RequestReviewReturnProductDTO requestReviewReturnDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorReturnService.ReviewReturnOrderProduct(requestReviewReturnDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("CreateReturnRefund/{returnId}")]
    public async Task<ActionResult> CreateReturnRefund(int returnId)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorReturnService.AcceptReturnProduct(returnId, vendorUserId);
        return Ok(result);
    }
    [HttpGet("returns")]
    public async Task<IActionResult> GetAllReturnsForVendor([FromQuery] RequestVendorReturnFilter request)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _vendorReturnService.GetAllReturnsForVendor(request, vendorUserId);
        return Ok(result);
    }

    [HttpGet("{returnId}")]
    [Authorize]
    public async Task<IActionResult> GetReturnDetails(int returnId)
    {
        var result = await _vendorReturnService.GetReturnDetails(returnId);
        return Ok(result);
    }
}