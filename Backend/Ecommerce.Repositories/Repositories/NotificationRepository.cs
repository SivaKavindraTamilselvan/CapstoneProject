using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class NotificationRepsository : AbstractRepository<int, Notification>, INotificationRepsository
{
    public NotificationRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

    public async Task<(List<Notification> items, int totalCount)> GetAllNotifcationByUserId(NotificationFilter request, int userId)
    {
        var query = _ecommerceContext.Notification.Include(n => n.NotificationType).Where(n => n.UserId == userId);
        if (request.MaxCreatedAt.HasValue)
        {
            query = query.Where(n => n.CreatedAt <= request.MaxCreatedAt.Value);
        }
        if (request.MinCreatedAt.HasValue)
        {
            query = query.Where(n => n.CreatedAt >= request.MinCreatedAt.Value);
        }
        if (request.IsRead.HasValue)
        {
            query = query.Where(n => n.IsRead == request.IsRead.Value);
        }
        if (request.NotificationTypeId.HasValue)
        {
            query = query.Where(n => n.NotificationTypeId == request.NotificationTypeId.Value);
        }
        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.CreatedAt).Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        return (items, totalCount);
    }

    public async Task ClearAllNotificationsByUserId(int userId)
    {
        var notifications = await _ecommerceContext.Notification.Where(n => n.UserId == userId).ToListAsync();
        _ecommerceContext.Notification.RemoveRange(notifications);
        await _ecommerceContext.SaveChangesAsync();
    }
}