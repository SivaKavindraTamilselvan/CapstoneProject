using Ecommerce.DTOs;
using Ecommerce.DTOs.Shipment;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ShiprocketController : ControllerBase
{
    private readonly IShipmentService _shipmentService;
    private readonly IShipRocketService _shiprocketService;
    private readonly IAdminShipmentService _adminShipmentService;

    public ShiprocketController(IShipmentService shipmentService,IShipRocketService shiprocketService,IAdminShipmentService adminShipmentService)
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

    [HttpPut("updateShipmentStatus")]

    public async Task<IActionResult> UpdateShipmentStatus(ShipmentStatusRequestDTO shipmentStatusRequestDTO)
    {
        var result = await _adminShipmentService.UpdateShimentStatus(shipmentStatusRequestDTO);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllShipmentsForAdmin([FromQuery] ShipmentFilterDto filter)
    {
        var result = await _shipmentService.GetAllShipmentsForAdmin(filter);
        return Ok(result);
    }

    [HttpGet("{shipmentId}")]
    public async Task<IActionResult> GetShipmentDetailForAdmin(int shipmentId)
    {
        var result = await _shipmentService.GetShipmentDetailForAdmin(shipmentId);
        return Ok(result);
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetShipmentsByOrderForAdmin(int orderId)
    {
        var result = await _shipmentService.GetShipmentsByOrderForAdmin(orderId);
        return Ok(result);
    }

    [HttpGet("tracking/{trackingNumber}")]
    public async Task<IActionResult> GetShipmentByTrackingForAdmin(string trackingNumber)
    {
        var result = await _shipmentService.GetShipmentByTrackingForAdmin(trackingNumber);
        return Ok(result);
    }
}