using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly IUserOrderService _userOrderService;
    private readonly IVendorOrderService _vendorOrderService;
    private readonly IUserReturnService _userReturnService;

    public OrderController(IOrderService orderService,IUserReturnService userReturnService, IUserOrderService userOrderService, IVendorOrderService vendorOrderService)
    {
        _orderService = orderService;
        _userOrderService = userOrderService;
        _vendorOrderService = vendorOrderService;
        _userReturnService = userReturnService;
    }
    [Authorize]
    [HttpPost("AddOrder")]
    public async Task<ActionResult<ResponseAddOrderDTO>> AddOrder(RequestAddOrderDTO requestAddOrderDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userOrderService.AddOrder(requestAddOrderDTO, UserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndOrderVendorOnly")]
    [HttpGet("GetOrder")]
    public async Task<ActionResult<List<ResponseGetOrderItems>>> GetOrder()
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _vendorOrderService.GetAllTheActiveOrder(vendorId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndOrderVendorOnly")]
    [HttpPut("UpdateOrderStatus")]
    public async Task<ActionResult<ResponseGetOrderItems>> UpdateOrderStatus(int orderitemid)
    {
        var result = await _vendorOrderService.UpdateTheOrderStatus(orderitemid);
        return Ok(result);
    }
    [Authorize]
    [HttpPost("RequestReturnOrder")]
    public async Task<ActionResult<ResponseAddReturnDTO>> RequestAddReturn(RequestAddReturnDTO requestAddReturnDTO)
    {
        var result = await _userReturnService.AddReturn(requestAddReturnDTO);
        return Ok(result);
    }
    [HttpGet("admin")]
    public async Task<IActionResult> GetOrdersAdmin([FromQuery] OrderFilterParams filters)
    {
        var result = await _orderService.GetOrderByAdmin(filters);
        return Ok(result);
    }
    [HttpGet("vendor")]
    public async Task<IActionResult> GetOrdersVendor([FromQuery] OrderFilterParams filters)
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _orderService.GetOrderByVendor(filters,vendorId);
        return Ok(result);
    }
    [HttpGet("user")]
    public async Task<IActionResult> GetOrdersUser([FromQuery] OrderFilterParams filters)
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _orderService.GetOrderByUserId(filters,vendorId);
        return Ok(result);
    }
}