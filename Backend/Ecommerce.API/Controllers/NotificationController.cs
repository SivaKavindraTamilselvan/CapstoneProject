using Ecommerce.API.Hubs;
using Ecommerce.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationController(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] string message)
    {
        await _hubContext.Clients
            .Group("2")  // ← User 2 who is connected
            .SendAsync("ReceiveNotification", new
            {
                message = message,
                orderId = 999
            });

        return Ok("Notification sent");
    }
}