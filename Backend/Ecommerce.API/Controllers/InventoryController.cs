using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPost("AddInventory")]
    public async Task<ActionResult<ResponseAddInventoryDTO>> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.AddInventory(requestAddInventoryDTO);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndInventoryVendorOnly")]
    [HttpPost("UpdateInventory")]
    public async Task<ActionResult<ResponseUpdateInventoryDTO>> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.UpdateInventory(requestUpdateInventoryDTO);
        return Ok(result);
    }
}