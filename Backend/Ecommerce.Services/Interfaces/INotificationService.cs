// Ecommerce.Services/Interfaces/INotificationService.cs
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
    }
}