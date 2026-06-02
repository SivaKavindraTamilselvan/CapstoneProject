using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserCartService _userCartService;
    private readonly IUserFavoriteService _userFavoriteService;
    public UserController(IUserCartService userCartService,IUserFavoriteService userFavoriteService)
    {
        _userCartService = userCartService;
        _userFavoriteService = userFavoriteService;
    }

    [Authorize]
    [HttpPost("AddToFavorite")]
    public async Task<ActionResult<ResponseAddFavoriteItemsDTO>> AddToFavorite(RequestAddFavoriteItemsDTO requestAddFavoriteItemsDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userFavoriteService.AddFavorite(requestAddFavoriteItemsDTO,UserId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("AddToCart")]
    public async Task<ActionResult<ResponseAddCartItemsDTO>> AddToCart(RequestAddCartItemsDTO requestAddCartItemsDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _userCartService.AddCart(requestAddCartItemsDTO,UserId);
        return Ok(result);
    }
}