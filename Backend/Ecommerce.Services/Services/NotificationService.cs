using Ecommerce.API.Hubs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepsository _notificationRepsository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        INotificationRepsository notificationRepsository)
    {
        _notificationRepsository = notificationRepsository;
        _hubContext = hubContext;
    }

    public async Task SendToUser(
        int userId,
        string title,
        string message,
        int notificationTypeId,
        string? referenceType = null,
        int? referenceId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            Title = title,
            Message = message,
            NotificationTypeId = notificationTypeId,
            ReferenceType = referenceType,
            ReferenceId = referenceId,
            IsRead = false,
            CreatedAt = DateTime.Now
        };

        await _notificationRepsository.Create(notification);

        await _hubContext.Clients
            .Group(userId.ToString())
            .SendAsync("ReceiveNotification", new
            {
                notification.NotificationId,
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.NotificationTypeId,
                notification.ReferenceType,
                notification.ReferenceId,
                notification.IsRead,
                notification.CreatedAt
            });
    }
}