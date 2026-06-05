using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ShiprocketController : ControllerBase
{
    private readonly IShipRocketService _shiprocketService;
    private readonly IAdminShipmentService _adminShipmentService;

    public ShiprocketController(IShipRocketService shiprocketService,IAdminShipmentService adminShipmentService)
    {
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
}