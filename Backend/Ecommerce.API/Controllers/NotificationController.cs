using Ecommerce.API.Hubs;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationController(IHubContext<NotificationHub> hubContext,INotificationService notificationService)
    {
        _hubContext = hubContext;
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] RequestSendNotificationDTO request)
    {
        await _notificationService.SendToUser(
            request.UserId.ToString(),
            new
            {
                message = request.Message,
                status = request.Status
            }
        );

        return Ok("Notification sent");
    }
}