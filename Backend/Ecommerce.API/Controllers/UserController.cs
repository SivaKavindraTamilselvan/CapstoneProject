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
    private readonly IUserFavoriteService _userFavoriteService;
    private readonly IAddressService _addressService;
    private readonly IReviewService _reviewService;
    public UserController(IUserFavoriteService userFavoriteService, IAddressService addressService, IReviewService reviewService)
    {
        _userFavoriteService = userFavoriteService;
        _addressService = addressService;
        _reviewService = reviewService;
    }
    [Authorize]
    [HttpPost("AddAdress")]
    public async Task<ActionResult<ResponseAddAddressDTO>> AddAddress(RequestAddAddressDTO requestAddAddressDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.AddAddress(requestAddAddressDTO, UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpPut("MakeAdressAsDefault")]
    public async Task<ActionResult<ResponseMakeDefaultAddressDTO>> UpdateAddressAsDefault(RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.MakeAddressDefault(requestMakeDefaultAddressDTO, UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("ActiveAdress")]
    public async Task<ActionResult<ResponseGetAddressDTO>> GetAddress([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.GetAllActiveAddress(UserId, pageNumber, pageSize);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("AllVendorAdress")]
    public async Task<ActionResult<ResponseGetAddressDTO>> GetAllVendorAddress([FromQuery] int pageNumber = 1, [FromQuery] bool? status = null,[FromQuery] int pageSize = 10)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.GetAllTheVendorAddress(UserId, status, pageNumber, pageSize);
        return Ok(result);
    }
    [Authorize]
    [HttpPatch("Adress")]
    public async Task<ActionResult<ResponseGetAddressDTO>> DeleteAddress([FromQuery]int addressId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.DeleteUserAddress(addressId,UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpPatch("VendorAdress")]
    public async Task<ActionResult<ResponseGetAddressDTO>> DeleteVendorAddress([FromQuery]int addressId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.DeleteInventoryAddress(addressId,UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpPost("AddReview")]
    public async Task<ActionResult<ResponseAddReviewDTO>> AddReview(RequestAddReviewDTO requestAddReviewDTO)
    {
        var result = await _reviewService.AddReview(requestAddReviewDTO);
        return Ok(result);
    }
}