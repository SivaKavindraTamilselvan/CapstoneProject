using System.Security.Claims;
using Ecommerce.DTOs;
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
    public UserController(IUserFavoriteService userFavoriteService,IAddressService addressService)
    {
        _userFavoriteService = userFavoriteService;
        _addressService = addressService;
    }
    [Authorize]
    [HttpPost("AddAdress")]
    public async Task<ActionResult<ResponseAddAddressDTO>> AddAddress(RequestAddAddressDTO requestAddAddressDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.AddAddress(requestAddAddressDTO,UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpPost("MakeAdressAsDefault")]
    public async Task<ActionResult<ResponseMakeDefaultAddressDTO>> UpdateAddressAsDefault(RequestMakeDefaultAddressDTO requestMakeDefaultAddressDTO)
    {
        var result = await _addressService.MakeAddressDefault(requestMakeDefaultAddressDTO);
        return Ok(result);
    }
}