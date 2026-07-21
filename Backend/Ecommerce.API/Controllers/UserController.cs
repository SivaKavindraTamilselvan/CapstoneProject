using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserOrderService _userOrderService;
    private readonly ICancelService _userCancelService;
    private readonly IUserReturnService _userReturnService;
    private readonly IReviewService _reviewService;
    public UserController(IUserOrderService userOrderService,ICancelService userCancelService, IUserReturnService userReturnService, IUserFavoriteService userFavoriteService, IAddressService addressService, IReviewService reviewService)
    {
        _userOrderService = userOrderService;
        _userCancelService = userCancelService;
        _userReturnService = userReturnService;
        _reviewService = reviewService;
    }
    
    [Authorize]
    [HttpPost("AddReview")]
    public async Task<ActionResult<ResponseAddReviewDTO>> AddReview(RequestAddReviewDTO requestAddReviewDTO)
    {
        var result = await _reviewService.AddReview(requestAddReviewDTO);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("returns")]
    public async Task<IActionResult> GetAllReturnsForUser([FromQuery] RequestUserReturnFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userReturnService.GetAllReturnsForUser(request, UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("user-cancels")]
    public async Task<IActionResult> GetAllCancelsForUser([FromQuery] RequestUserCancelFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.GetAllCancelsForUser(request,UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("wallet-balance")]
    public async Task<IActionResult> GetWalletBalance()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userOrderService.GetWalletBalance(UserId);
        return Ok(result);
    }
}