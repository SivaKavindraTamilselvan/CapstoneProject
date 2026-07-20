using System.Security.Claims;
using Ecommerce.DTOs;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ShiprocketController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    private readonly IShipRocketService _shiprocketService;
    private readonly IAdminShipmentService _adminShipmentService;

    public ShiprocketController(IShipmentService shipmentService, IShipRocketService shiprocketService, IAdminShipmentService adminShipmentService)
    {
        _shipmentService = shipmentService;
        _shiprocketService = shiprocketService;
        _adminShipmentService = adminShipmentService;
    }

    [HttpPost("serviceability")]
    public async Task<IActionResult> CheckServiceability(ServiceabilityRequestDTO request)
    {
        var result = await _shiprocketService
            .CheckServiceability(request);

        return Ok(result);
    }

    [Authorize(Policy = "CouponLogisticAdminOrSuperAdminOnly")]
    [HttpPut("updateShipmentStatus")]
    public async Task<IActionResult> UpdateShipmentStatus(ShipmentStatusRequestDTO shipmentStatusRequestDTO)
    {
        int adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _adminShipmentService.UpdateShimentStatus(shipmentStatusRequestDTO, adminUserId);
        return Ok(result);
    }

    [Authorize(Policy = "CouponLogisticAdminOrSuperAdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAllShipmentsForAdmin([FromQuery] RequestShipmentFilter filter)
    {
        var result = await _shipmentService.GetAllShipmentsForAdmin(filter);
        return Ok(result);
    }

    [Authorize(Policy = "CouponLogisticAdminOrSuperAdminOnly")]
    [HttpGet("{shipmentId}")]
    public async Task<IActionResult> GetShipmentDetailForAdmin([FromRoute] int shipmentId)
    {
        var result = await _shipmentService.GetShipmentDetailForAdmin(shipmentId);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("user/{orderItemId}")]
    public async Task<IActionResult> GetShipmentDetailForUser([FromRoute] int orderItemId)
    {
        var result = await _shipmentService.GetShipmentDetailForOrderItemId(orderItemId);
        return Ok(result);
    }
}