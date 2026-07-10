using Ecommerce.Data;
using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories.Interfaces;

public class NotificationRepsository : AbstractRepository<int, Notification>, INotificationRepsository
{
    public NotificationRepsository(EcommerceContext ecommerceContext) : base(ecommerceContext)
    {

    }

}