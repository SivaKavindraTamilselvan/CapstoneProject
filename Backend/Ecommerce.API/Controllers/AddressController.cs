using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressController : ControllerBase
{
    private readonly IAddressService _addressService;
    public AddressController(IAddressService addressService)
    {
        _addressService = addressService;
    }

    [Authorize]
    [HttpPost("add-address")]
    public async Task<ActionResult<ResponseAddAddressDTO>> AddAddress(RequestAddAddressDTO requestAddAddressDTO)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.AddAddress(requestAddAddressDTO, UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpPut("{addressId}/default")]
    public async Task<ActionResult<ResponseMakeDefaultAddressDTO>> UpdateAddressAsDefault(int addressId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.MakeAddressDefault(addressId, UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpGet("actice-address")]
    public async Task<ActionResult<ResponseGetAddressDTO>> GetUserAddress()
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.GetAllActiveUserAddress(UserId);
        return Ok(result);
    }
    [Authorize]
    [HttpPatch("{addressId}/deactivate")]
    public async Task<ActionResult<ResponseGetAddressDTO>> DeleteAddress([FromRoute] int addressId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.DeleteUserAddress(addressId, UserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerAndInventoryVendorOnly")]
    [HttpGet("vendor-address")]
    public async Task<ActionResult<PagedResponse<ResponseGetAddressDTO>>> GetAllVendorAddress([FromQuery] AddressRequestFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.GetAllTheVendorAddress(UserId, request);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOwnerAndInventoryVendorOnly")]
    [HttpPatch("vendor/{addressId}/deactivate")]
    public async Task<ActionResult<ResponseGetAddressDTO>> DeleteVendorAddress([FromQuery] int addressId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.DeleteInventoryAddress(addressId, UserId);
        return Ok(result);
    }
}