using System.Security.Claims;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly IUserCartService _userCartService;
    public CartController(IUserCartService userCartService)
    {
        _userCartService = userCartService;
    }
    [Authorize]
    [HttpPost("AddToCart")]
    public async Task<ActionResult<ResponseCartItemsDTO>> AddToCart(RequestAddCartItemsDTO requestAddCartItemsDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCartService.AddCart(requestAddCartItemsDTO, UserId);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("UpdateToCart")]
    public async Task<ActionResult<ResponseCartItemsDTO>> UpdateToCart(RequestUpdateCartItemsDTO requestUpdateCartItemsDTO)
    {
        var result = await _userCartService.UpdateCart(requestUpdateCartItemsDTO);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("DeleteCartItem")]
    public async Task<ActionResult<ResponseCartItemsDTO>> DeleteCartItem(RequestDeleteCartItemsDTO requestDeleteCartItemsDTO)
    {
        var result = await _userCartService.DeleteCart(requestDeleteCartItemsDTO);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("DeleteAllCartItem")]
    public async Task<ActionResult<ResponseCartItemsDTO>> DeleteAllCartItem()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCartService.DeleteAllCart(UserId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetCartByUserId")]
    public async Task<ActionResult<List<ResponseGetCartDTO>>> GetCartByUserId()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCartService.GetCartByUserId(UserId);
        return Ok(result);
    }
}