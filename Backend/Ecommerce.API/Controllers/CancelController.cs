using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CancelController : ControllerBase
{
    private readonly ICancelService _userCancelService;
    public CancelController(ICancelService userCancelService)
    {
        _userCancelService = userCancelService;

    }

    [Authorize(Policy = "OrderAdminOrSuperAdminOnly")]
    [HttpGet("admin-cancels")]
    public async Task<IActionResult> GetAllCancelsForAdmin([FromQuery] RequestAdminCancelFilter request)
    {
        var result = await _userCancelService.GetAllCancelsForAdmin(request);
        return Ok(result);
    }

    [Authorize(Policy = "OrderAdminOrSuperAdminOnly")]
    [HttpGet("admin-cancel/{cancelId}")]
    public async Task<IActionResult> GetCancelsForAdmin(int cancelId)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.GetAllCancel(cancelId, vendorUserId);
        return Ok(result);
    }

    [Authorize(Policy = "VendorOnwerAndOrderVendorOnly")]
    [HttpGet("vendor-cancels")]
    public async Task<IActionResult> GetAllCancelsForVendor([FromQuery] RequestVendorCancelFilter request)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.GetAllCancelsForVendor(request, vendorUserId);
        return Ok(result);
    }

    [Authorize(Policy = "VendorOnwerAndOrderVendorOnly")]
    [HttpGet("vendor-cancel/{cancelId}")]
    public async Task<IActionResult> GetCancelsForVendor(int cancelId)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.GetAllCancel(cancelId, vendorUserId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("request-cancels")]
    public async Task<IActionResult> RequestCancels(RequestCancelDTO request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.RequestCancel(request, UserId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("user-cancels")]
    public async Task<IActionResult> GetAllCancelsForUser([FromQuery] RequestUserCancelFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.GetAllCancelsForUser(request, UserId);
        return Ok(result);
    }

}