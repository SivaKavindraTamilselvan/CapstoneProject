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

    [HttpGet("pincode/{pincode}")]
    public async Task<IActionResult> GetPincode(string pincode)
    {
        using var client = new HttpClient();
        var response = await client.GetStringAsync($"https://api.postalpincode.in/pincode/{pincode}");
        return Content(response, "application/json");
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
    [HttpGet("address/{addressId}")]
    public async Task<ActionResult<ResponseGetAddressDTO>> GetUserAddress(int addressId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.GetAddress(UserId, addressId);
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

    [Authorize(Policy = "VendorOnwerAndInventoryVendorOnly")]
    [HttpGet("vendor-address")]
    public async Task<ActionResult<PagedResponse<ResponseGetAddressDTO>>> GetAllVendorAddress([FromQuery] AddressRequestFilter request)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.GetAllTheVendorAddress(UserId, request);
        return Ok(result);
    }
    
    [Authorize(Policy = "VendorOnwerAndInventoryVendorOnly")]
    [HttpPatch("vendor/{addressId}/deactivate")]
    public async Task<ActionResult<ResponseGetAddressDTO>> DeleteVendorAddress([FromRoute] int addressId)
    {
        int UserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _addressService.DeleteInventoryAddress(addressId, UserId);
        return Ok(result);
    }
}