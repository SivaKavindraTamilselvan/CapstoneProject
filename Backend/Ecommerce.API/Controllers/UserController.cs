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
    private readonly ICancelService _userCancelService;
    private readonly IUserReturnService _userReturnService;
    private readonly IUserFavoriteService _userFavoriteService;
    private readonly IAddressService _addressService;
    private readonly IReviewService _reviewService;
    public UserController(ICancelService userCancelService, IUserReturnService userReturnService, IUserFavoriteService userFavoriteService, IAddressService addressService, IReviewService reviewService)
    {
        _userCancelService = userCancelService;
        _userReturnService = userReturnService;
        _userFavoriteService = userFavoriteService;
        _addressService = addressService;
        _reviewService = reviewService;
    }
    
    [Authorize]
    [HttpPost("AddReview")]
    public async Task<ActionResult<ResponseAddReviewDTO>> AddReview(RequestAddReviewDTO requestAddReviewDTO)
    {
        var result = await _reviewService.AddReview(requestAddReviewDTO);
        return Ok(result);
    }
    [HttpGet("returns")]
    public async Task<IActionResult> GetAllReturnsForUser([FromQuery] RequestUserReturnFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userReturnService.GetAllReturnsForUser(request, UserId);
        return Ok(result);
    }
    [HttpGet("admin-cancels")]
    public async Task<IActionResult> GetAllCancelsForAdmin([FromQuery] RequestAdminCancelFilter request)
    {
        var result = await _userCancelService.GetAllCancelsForAdmin(request);
        return Ok(result);
    }
    [HttpGet("user-cancels")]
    public async Task<IActionResult> GetAllCancelsForUser([FromQuery] RequestUserCancelFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.GetAllCancelsForUser(request,UserId);
        return Ok(result);
    }
    [HttpGet("vendor-cancels")]
    public async Task<IActionResult> GetAllCancelsForVendor([FromQuery] RequestVendorCancelFilter request)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCancelService.GetAllCancelsForVendor(request,vendorUserId);
        return Ok(result);
    }
}