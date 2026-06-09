using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly IAdminProductAttributeService _adminProductAttributeService;
    private readonly IAdminProductCategoryService _adminProductCategoryService;
    private readonly IAdminService _adminService;
    private readonly IAdminProductService _adminProductService;
    private readonly IAdminVendorService _adminVendorService;
    private readonly IAdminReturnService _adminReturnService;
    private readonly IAdminRefundService _adminRefundService;
    public AdminController(IAdminRefundService adminRefundService, IAdminReturnService adminReturnService, IAdminProductAttributeService adminProductAttributeService, IAdminProductCategoryService adminProductCategoryService, IAdminService adminService, IAdminProductService adminProductService, IAdminVendorService adminVendorService)
    {
        _adminService = adminService;
        _adminProductService = adminProductService;
        _adminVendorService = adminVendorService;
        _adminProductAttributeService = adminProductAttributeService;
        _adminProductCategoryService = adminProductCategoryService;
        _adminReturnService = adminReturnService;
        _adminRefundService = adminRefundService;
    }

    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost("RegisterAdmin")]
    public async Task<ActionResult<ResponseRegisterUserDTO>> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.RegisterAdmin(requestRegisterAdminDTO, adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("ReviewVendor")]
    public async Task<ActionResult<ResponseReviewOfVendorDTO>> ReviewVendor(RequestReviewOfVendorDTO dto)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.ReviewVendor(dto, adminUserId);

        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("DeleteVendor")]
    public async Task<ActionResult<ResponseReviewOfVendorDTO>> DeleteVendor(int vendorId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminVendorService.DeleteVendor(vendorId,adminUserId);

        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpGet("GetVendor")]
    public async Task<ActionResult<List<ResponseGetVendor>>> GetVendor([FromQuery] int? statusId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _adminVendorService.GetVendor(statusId, pageNumber, pageSize);
        return Ok(result);
    }

    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("CreateReturnShipment")]
    public async Task<ActionResult> CreateShipmentForReturnProduct(int returnId)
    {
        var result = await _adminReturnService.CreateReturnShipment(returnId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("CreateRefund")]
    public async Task<ActionResult> UpdateRefund(RequestUpdateRefundDTO requestUpdateRefundDTO)
    {
        var result = await _adminRefundService.ReviewRefund(requestUpdateRefundDTO);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("ReviewRefund")]
    public async Task<ActionResult> CreateRefund(RequestAddRefundDTO requestAddRefundDTO)
    {
        var result = await _adminRefundService.CreateRefund(requestAddRefundDTO);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("CreateReturnRefund")]
    public async Task<ActionResult> CreateReturnRefund(RequestAddReturnRefundDTO requestAddRefundDTO)
    {
        var result = await _adminRefundService.CreateReturnRefund(requestAddRefundDTO);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpGet("GetAdminUsER")]
    public async Task<ActionResult<List<ResponseGetAdminUserDTO>>> GetAdminUser([FromQuery] int? roleId, [FromQuery] bool? status, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
    {
        var result = await _adminService.GetAllAdminUser(roleId, status, pageNumber, pageSize);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpGet("GetAdminUser/{adminUserId}")]
    public async Task<ActionResult<List<ResponseGetAdminUserDTO>>> GetAdminUserByAdminUserId(int adminUserId)
    {
        var result = await _adminService.GetAdminUserByUserId(adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPut("admin-users/{adminUserId}/deactivate")]
    public async Task<IActionResult> DeactivateAdminUser(int adminUserId)
    {
        var result = await _adminService.DeactivateAdminUser(adminUserId);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPut("admin-users/{adminUserId}/activate")]
    public async Task<IActionResult> ActivateAdminUser(int adminUserId)
    {
        var result = await _adminService.ActivateAdminUser(adminUserId);
        return Ok(result);
    }
}