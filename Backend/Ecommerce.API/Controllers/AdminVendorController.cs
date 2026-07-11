using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
public class AdminVendorController : ControllerBase
{
    private readonly IAdminVendorService _adminVendorService;
    public AdminVendorController(IAdminVendorService adminVendorService)
    {
        _adminVendorService = adminVendorService;
    }
    [HttpPut("ReviewVendor")]
    public async Task<ActionResult<ResponseReviewOfVendorDTO>> ReviewVendor(RequestReviewOfVendorDTO dto)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.ReviewVendor(dto, adminUserId);

        return Ok(result);
    }
    [HttpPatch("DeleteVendor")]
    public async Task<ActionResult<ResponseReviewOfVendorDTO>> DeleteVendor(DeleteVendorDto deleteVendorDto)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.DeleteVendor(deleteVendorDto, adminUserId);
        return Ok(result);
    }
    [HttpGet("GetVendor")]
    public async Task<ActionResult<PagedResponse<ResponseGetVendor>>> GetVendor([FromQuery] RequestAdminVendorFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.GetVendorsForAdmin(request, adminUserId);
        return Ok(result);
    }
    [HttpGet("GetVendor/{vendorId}")]
    public async Task<ActionResult<List<ResponseGetVendor>>> GetVendorByVendorId([FromRoute] int vendorId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.GetVendorsByVendorIdForAdmin(vendorId, adminUserId);
        return Ok(result);
    }

    [HttpGet("GetVendorUser")]
    public async Task<ActionResult<List<ResponseGetVendor>>> GetVendorUserByVendorId([FromQuery] RequestAdminVendorUserFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.GetVendorUserByVendorIdForAdmin(request, adminUserId);
        return Ok(result);
    }
}