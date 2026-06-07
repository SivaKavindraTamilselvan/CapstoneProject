// Ecommerce.API/Services/NotificationService.cs
using Ecommerce.API.Hubs;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendToUser(string userId, object payload)
        {
            await _hubContext.Clients
                .Group(userId)
                .SendAsync("ReceiveNotification", payload);
        }
    }
}