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
    private readonly IAdminInventoryService _adminInventoryService;
    private readonly IInventoryService _inventoryService;
    public InventoryController(IInventoryService inventoryService, IAdminInventoryService adminInventoryService)
    {
        _adminInventoryService = adminInventoryService;
        _inventoryService = inventoryService;
    }
    [Authorize(Policy = "VendorOwnerOnly")]
    [HttpPost("AddInventory")]
    public async Task<ActionResult<ResponseAddInventoryDTO>> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.AddInventory(requestAddInventoryDTO, vendorUserId);
        return Ok(result);
    }
    [Authorize(Policy = "VendorOnwerAndInventoryVendorOnly")]
    [HttpPost("UpdateInventory")]
    public async Task<ActionResult<ResponseUpdateInventoryDTO>> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.UpdateInventory(requestUpdateInventoryDTO, vendorUserId);
        return Ok(result);
    }
    [HttpGet("inventories")]
    public async Task<ActionResult> GetInventoryForAdmin([FromQuery] RequestAdminInventoryFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminInventoryService.GetInventory(request, adminUserId);
        return Ok(result);
    }
    [HttpGet("inventories/{inventoryId}")]
    public async Task<ActionResult> GetInventoryForAdminById([FromRoute] int inventoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminInventoryService.GetInventoryById(inventoryId, adminUserId);
        return Ok(result);
    }

    [HttpGet("vendor-inventories")]
    public async Task<ActionResult> GetInventoryForVendor([FromQuery] RequestVendorInventoryFilter request)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.GetInventory(request, adminUserId);
        return Ok(result);
    }
    [HttpGet("vendor-inventories/{inventoryId}")]
    public async Task<ActionResult> GetInventoryForVendorById([FromRoute] int inventoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.GetInventoryById(inventoryId, adminUserId);
        return Ok(result);
    }
    [HttpPut("vendor-inventories/{inventoryId}")]
    public async Task<ActionResult> DeleteInventoryForVendorById([FromRoute] int inventoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.DeleteInventory(inventoryId, adminUserId);
        return Ok(result);
    }


}