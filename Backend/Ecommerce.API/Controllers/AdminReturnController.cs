using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "ReturnAdminOrSuperAdminOnly")]
public class AdminReturnController : ControllerBase
{
    private readonly IAdminReturnService _adminReturnService;
    private readonly IAdminRefundService _adminRefundService;
    public AdminReturnController(IAdminRefundService adminRefundService, IAdminReturnService adminReturnService)
    {
        _adminReturnService = adminReturnService;
        _adminRefundService = adminRefundService;
    }

    [HttpGet("returns/{returnId}")]
    public async Task<IActionResult> GetAllReturns(int returnId)
    {
        var result = await _adminReturnService.GetAllReturns(returnId);
        return Ok(result);
    }

    [HttpPost("CreateReturnRefund")]
    public async Task<ActionResult> CreateReturnRefund(RequestAddReturnRefundDTO requestAddRefundDTO)
    {
        var result = await _adminRefundService.CreateReturnRefund(requestAddRefundDTO);
        return Ok(result);
    }

    [HttpGet("returns")]
    public async Task<IActionResult> GetAllReturnsForAdmin([FromQuery] RequestAdminReturnFilter request)
    {
        var result = await _adminReturnService.GetAllReturnsForAdmin(request);
        return Ok(result);
    }

}