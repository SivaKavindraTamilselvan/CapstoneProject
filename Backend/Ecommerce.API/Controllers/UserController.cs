using System.Security.Claims;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserCartService _userCartService;
    private readonly IUserFavoriteService _userFavoriteService;
    public UserController(IUserCartService userCartService, IUserFavoriteService userFavoriteService)
    {
        _userCartService = userCartService;
        _userFavoriteService = userFavoriteService;
    }

    [Authorize]
    [HttpPost("AddToFavorite")]
    public async Task<ActionResult<ResponseFavoriteItemsDTO>> AddToFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userFavoriteService.AddFavorite(requestAddFavoriteItemsDTO, UserId);
        return Ok(result);
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
}