using System.Security.Claims;
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
    private readonly IUserOrderService _userOrderService;

    public OrderController(IUserOrderService userOrderService)
    {
        _userOrderService = userOrderService;
    }
    [Authorize]
    [HttpPost("AddOrder")]
    public async Task<ActionResult<ResponseAddOrderDTO>> AddOrder(RequestAddOrderDTO requestAddOrderDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userOrderService.AddOrder(requestAddOrderDTO,UserId);
        return Ok(result);
    }
}