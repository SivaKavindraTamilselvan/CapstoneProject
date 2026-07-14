using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Services.Services;

public class DemandNudgeBackgroundService : BackgroundService
{
    private readonly ILogger<DemandNudgeBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IConfiguration _configuration;

    public DemandNudgeBackgroundService(
        ILogger<DemandNudgeBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = _configuration.GetSection("DemandNudgeSettings");
        int pollingInterval = settings.GetValue<int>("PollingIntervalMinutes", 60);

        _logger.LogInformation($"DemandNudgeBackgroundService starting. Polling every {pollingInterval} minutes.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDemandNudgesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing demand nudges.");
            }

            await Task.Delay(TimeSpan.FromMinutes(pollingInterval), stoppingToken);
        }
    }

    private async Task ProcessDemandNudgesAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<EcommerceContext>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        var settings = _configuration.GetSection("DemandNudgeSettings");

        int minimumWaitTime = settings.GetValue<int>("MinimumWaitTimeMinutes", 1); // 1 minute for demo
        decimal priceDropPercentage = settings.GetValue<decimal>("PriceDropPercentage", 10m);
        int lowStockThreshold = settings.GetValue<int>("LowStockThreshold", 5);
        int maxNotifications = settings.GetValue<int>("MaxNotificationsPerItem", 3);

        var thresholdTime = DateTime.SpecifyKind(DateTime.Now.AddMinutes(-minimumWaitTime), DateTimeKind.Unspecified);
        var oneMinAgo = DateTime.SpecifyKind(DateTime.Now.AddMinutes(-1), DateTimeKind.Unspecified);
        var nowUnspecified = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        // Fetch eligible items
        var eligibleItems = await context.FavoritesItems
            .Include(f => f.ProductVariant)
                .ThenInclude(pv => pv!.Product)
            .Include(f => f.Favorites)
                .ThenInclude(fv => fv!.Users)
            .Where(f => f.CreatedAt <= thresholdTime
                        && f.NotificationCount < maxNotifications
                        && (f.LastNotifiedAt == null || f.LastNotifiedAt <= oneMinAgo))
            .ToListAsync();

        if (eligibleItems.Count > 0)
        {
            _logger.LogInformation($"[DemandNudge] Processing {eligibleItems.Count} eligible wishlist items.");
        }

        foreach (var item in eligibleItems)
        {
            if (item.ProductVariant == null || item.Favorites?.Users == null)
            {
                _logger.LogWarning($"[DemandNudge] Skipping FavoritesItemId {item.FavoritesItemsId} due to null ProductVariant or User.");
                continue;
            }

            var user = item.Favorites.Users;
            var product = item.ProductVariant.Product;
            
            // Get inventory (total available)
            var totalAvailableQuantity = await context.Inventory
                .Where(i => i.ProductVariantId == item.ProductVariantId)
                .SumAsync(i => i.AvailableQuantity);

            bool sendNudge = false;
            string nudgeMessage = "";
            string nudgeTitle = "";

            int notifTypeId = 1;
            string iconHtml = "🔔";

            // Check Low Stock
            if (totalAvailableQuantity > 0 && totalAvailableQuantity <= lowStockThreshold)
            {
                sendNudge = true;
                nudgeTitle = "Low Stock Alert: Only a few left!";
                nudgeMessage = $"The item '{product?.ProductName}' in your wishlist is running low on stock. Only {totalAvailableQuantity} left! Get it before it's gone.";
                notifTypeId = 21; // LowStockAlert
                iconHtml = "⚠️";
            }
            // Check Price Drop
            else if (item.ProductVariant.Price <= item.LastNotifiedPrice * (1m - priceDropPercentage / 100m))
            {
                sendNudge = true;
                nudgeTitle = "Price Dropped!";
                nudgeMessage = $"Good news! The price for '{product?.ProductName}' has dropped to ₹{item.ProductVariant.Price}. Grab it now!";
                notifTypeId = 1; // Fallback
                iconHtml = "📉";
            }
            // Check General Reminder (1 min for demo). for 7 days DateTime.UtcNow.AddDays(-7)
            else if (item.CreatedAt <= oneMinAgo && item.NotificationCount == 0)
            {
                sendNudge = true;
                nudgeTitle = "Still thinking about it?";
                nudgeMessage = $"You've had '{product?.ProductName}' in your wishlist for a while. This item is in high demand, grab yours before it's gone!";
                notifTypeId = 1; 
                iconHtml = "🕒";
            }

            if (sendNudge)
            {
                _logger.LogInformation($"[DemandNudge] Sending '{nudgeTitle}' notification & email to {user.Email} (using nandhiraja16@gmail.com for test) for product '{product?.ProductName}'.");

                // In-App Notification
                await notificationService.SendToUser(user.UserId, nudgeTitle, nudgeMessage, notifTypeId, "ProductVariant", item.ProductVariantId);

                // Build a nice HTML email
                string frontendBaseUrl = _configuration["AppSettings:FrontendBaseUrl"] ?? "http://localhost:5173";
                string productUrl = $"{frontendBaseUrl}/user/product-details/{product?.ProductId}";
                
                string htmlEmail = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; border: 1px solid #e5e7eb; border-radius: 8px; padding: 24px; box-shadow: 0 4px 6px rgba(0,0,0,0.1);'>
                    <div style='text-align: center; margin-bottom: 24px;'>
                        <span style='font-size: 48px;'>{iconHtml}</span>
                        <h2 style='color:#1e293b; margin-top: 10px;'>{nudgeTitle}</h2>
                    </div>
                    <p style='color: #374151; font-size: 16px; line-height: 1.5;'>Hi {user.FirstName},</p>
                    <p style='color: #374151; font-size: 16px; line-height: 1.5;'>{nudgeMessage}</p>
                    
                    <div style='text-align: center; margin-top: 32px;'>
                        <a href='{productUrl}'
                           style='display:inline-block; background:#1e3a8a; color:#fff; padding:12px 32px;
                                  text-decoration:none; border-radius:6px; font-weight:bold; font-size: 16px;'>
                            View Item Now
                        </a>
                    </div>
                    <hr style='border: none; border-top: 1px solid #e5e7eb; margin-top: 32px; margin-bottom: 24px;' />
                    <p style='color:#6b7280; font-size:12px; text-align: center;'>
                        You received this because you have items saved in your wishlist. If you prefer not to receive these alerts, you can update your notification preferences.
                    </p>
                </div>";

                // Email Notification.  | user.Email
                await emailService.SendEmailAsync("nandhiraja16@gmail.com", nudgeTitle, htmlEmail);

                // Update state
                item.LastNotifiedPrice = item.ProductVariant.Price;
                item.LastNotifiedAt = nowUnspecified;
                item.NotificationCount++;
            }
        }

        await context.SaveChangesAsync();
    }
}
