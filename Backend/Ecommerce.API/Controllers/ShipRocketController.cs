using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ShiprocketController : ControllerBase
{
    private readonly IShipRocketService _shiprocketService;

    public ShiprocketController(IShipRocketService shiprocketService)
    {
        _shiprocketService = shiprocketService;
    }

    [HttpPost("serviceability")]
    public async Task<IActionResult> CheckServiceability(ServiceabilityRequestDTO request)
    {
        var result = await _shiprocketService
            .CheckServiceability(request);

        return Ok(result);
    }
}