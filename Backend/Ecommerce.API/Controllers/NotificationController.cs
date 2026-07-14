using System.Security.Claims;
using Ecommerce.API.Hubs;
using Ecommerce.DTOs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationController(IHubContext<NotificationHub> hubContext, INotificationService notificationService)
    {
        _hubContext = hubContext;
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] RequestSendNotificationDTO request)
    {
        /*
        await _notificationService.SendToUser(
            request.UserId.ToString(),
            new
            {
                message = request.Message,
            }
        );
        */

        return Ok("Notification sent");
    }

    [Authorize]
    [HttpGet("notifcation-list")]
    public async Task<IActionResult> GetAllNotificationByUserId([FromQuery] NotificationFilter request)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _notificationService.GetAllNotificationByUserId(request,userId);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("notifcation/{notificationId}")]
    public async Task<IActionResult> MarkAsRead([FromRoute] int notificationId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await _notificationService.UpdateNotification(notificationId,userId);
        return Ok(result);
    }

    [Authorize]
    [HttpDelete("notification/{notificationId}")]
    public async Task<IActionResult> DeleteNotification([FromRoute] int notificationId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _notificationService.DeleteNotification(notificationId, userId);
        return Ok("Notification deleted successfully");
    }

    [Authorize]
    [HttpDelete("notification/clear")]
    public async Task<IActionResult> ClearAllNotifications()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await _notificationService.ClearAllNotifications(userId);
        return Ok("All notifications cleared successfully");
    }
}