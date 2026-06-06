using System.Security.Claims;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FavoriteController : ControllerBase
{
    private readonly IUserFavoriteService _userFavoriteService;
    public FavoriteController(IUserFavoriteService userFavoriteService)
    {
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
    [HttpPut("DeleteFavoriteItem")]
    public async Task<ActionResult<ResponseFavoriteItemsDTO>> DeleteCartItem(RequestDeleteFavoriteItemsDTO requestDeleteFavoriteItemsDTO)
    {
        var result = await _userFavoriteService.DeleteFavorite(requestDeleteFavoriteItemsDTO);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("GetFavoriteByUserId")]
    public async Task<ActionResult<List<ResponseCartItemsDTO>>> GetCartByUserId()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userFavoriteService.GetFavoriteByUserId(UserId);
        return Ok(result);
    }
}
