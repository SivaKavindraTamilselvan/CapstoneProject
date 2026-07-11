using AutoMapper;
using Ecommerce.API.Hubs;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Services;

public class NotificationService : INotificationService
{
    private readonly IMapper _mapper;
    private readonly IUserValidation _userValidation;
    private readonly INotificationRepsository _notificationRepsository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        IHubContext<NotificationHub> hubContext,
        IUserValidation userValidation,
        IMapper mapper,
        INotificationRepsository notificationRepsository)
    {
        _mapper = mapper;
        _userValidation = userValidation;
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

    public async Task<PagedResponse<ResponseNotification>> GetAllNotificationByUserId(NotificationFilter request, int userId)
    {
        await _userValidation.ValidateUser(userId);
        var (items, totalCount) = await _notificationRepsository.GetAllNotifcationByUserId(request, userId);
        return new PagedResponse<ResponseNotification>
        {
            Items = _mapper.Map<List<ResponseNotification>>(items),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

    }
    public async Task<ResponseNotification> UpdateNotification(int notificationId, int userId)
    {
        await _userValidation.ValidateUser(userId);
        var notification = await _notificationRepsository.Get(notificationId);
        if (notification == null)
        {
            throw new DataNotFoundException("Notification is not found");
        }
        notification.IsRead = !notification.IsRead;
        notification.ReadAt = notification.IsRead ? DateTime.Now : null;
        var updatedNotification = await _notificationRepsository.Update(notification.NotificationId,notification);
        return _mapper.Map<ResponseNotification>(updatedNotification);
    }
}