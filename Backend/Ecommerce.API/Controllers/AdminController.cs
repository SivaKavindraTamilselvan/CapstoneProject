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
    [HttpPost("CreateReturnShipment")]
    public async Task<ActionResult> CreateShipmentForReturnProduct(int returnId)
    {
        var result = await _adminReturnService.CreateReturnShipment(returnId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPut("reviewRefund")]
    public async Task<ActionResult> UpdateRefund(RequestUpdateRefundDTO requestUpdateRefundDTO)
    {
        var result = await _adminRefundService.ReviewRefund(requestUpdateRefundDTO);
        return Ok(result);
    }
    [Authorize(Policy = "VendorAdminOrSuperAdminOnly")]
    [HttpPost("createRefund")]
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
    [HttpGet("GetAdminUser")]
    public async Task<ActionResult<List<ResponseGetAdminUserDTO>>> GetAdminUser([FromQuery]RequestAdiminUserFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.GetAllAdminUser(request,UserId);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpGet("GetAdminUser/{adminUserId}")]
    public async Task<ActionResult<List<ResponseGetAdminUserDTO>>> GetAdminUserByAdminUserId(int adminUserId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.GetAdminUserByUserId(adminUserId,UserId);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPut("admin-users/{adminUserId}/deactivate")]
    public async Task<IActionResult> DeactivateAdminUser(int adminUserId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.DeactivateAdminUser(adminUserId,UserId);
        return Ok(result);
    }
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPut("admin-users/{adminUserId}/activate")]
    public async Task<IActionResult> ActivateAdminUser(int adminUserId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.ActivateAdminUser(adminUserId,UserId);
        return Ok(result);
    }
    [HttpGet("returns")]
    public async Task<IActionResult> GetAllReturnsForAdmin([FromQuery] RequestAdminReturnFilter request)
    {
        var result = await _adminReturnService.GetAllReturnsForAdmin(request);
        return Ok(result);
    }

}