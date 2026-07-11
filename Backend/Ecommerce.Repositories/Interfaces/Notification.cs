using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface INotificationRepsository : IRepository<int, Notification>
{
    public Task<(List<Notification> items, int totalCount)> GetAllNotifcationByUserId(NotificationFilter request, int userId);
}