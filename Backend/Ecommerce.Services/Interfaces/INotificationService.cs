// Ecommerce.Services/Interfaces/INotificationService.cs
namespace Ecommerce.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendToUser(string userId, object payload);
    }
}