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
    private readonly IAdminService _adminService;
    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost("RegisterAdmin")]
    public async Task<ActionResult<ResponseRegisterUserDTO>> RegisterAdmin(RequestRegisterAdminDTO requestRegisterAdminDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminService.RegisterAdmin(requestRegisterAdminDTO, adminUserId);
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

}