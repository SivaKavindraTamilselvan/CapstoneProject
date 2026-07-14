// Ecommerce.Services/Interfaces/INotificationService.cs
using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendToUser(
            int userId,
            string title,
            string message,
            int notificationTypeId,
            string? referenceType = null,
            int? referenceId = null);
        public Task<PagedResponse<ResponseNotification>> GetAllNotificationByUserId(NotificationFilter request, int userId);
        public Task<ResponseNotification> UpdateNotification(int notificationId, int userId);
        public Task DeleteNotification(int notificationId, int userId);
        public Task ClearAllNotifications(int userId);
    }
}