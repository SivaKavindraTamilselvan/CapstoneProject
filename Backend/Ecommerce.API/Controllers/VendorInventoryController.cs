using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize(Policy = "VendorOnwerAndInventoryVendorOnly")]
public class VendorInventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;
    public VendorInventoryController(IInventoryService inventoryService, IAdminInventoryService adminInventoryService)
    {
        _inventoryService = inventoryService;
    }
    
    [HttpPost("add-inventory")]
    public async Task<ActionResult<ResponseAddInventoryDTO>> AddInventory(RequestAddInventoryDTO requestAddInventoryDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.AddInventory(requestAddInventoryDTO, vendorUserId);
        return Ok(result);
    }
    [HttpPut("update-inventory")]
    public async Task<ActionResult<ResponseUpdateInventoryDTO>> UpdateInventory(RequestUpdateInventoryDTO requestUpdateInventoryDTO)
    {
        int vendorUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.UpdateInventory(requestUpdateInventoryDTO, vendorUserId);
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
    [HttpPatch("vendor-inventories/{inventoryId}")]
    public async Task<ActionResult> DeleteInventoryForVendorById([FromRoute] int inventoryId)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _inventoryService.DeleteInventory(inventoryId, adminUserId);
        return Ok(result);
    }
}