using Ecommerce.Data;
using Ecommerce.Services.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Functions;

public class CheckAbandonedCarts
{
    private readonly EcommerceContext _context;
    private readonly INotificationService _notificationService;

    public CheckAbandonedCarts(EcommerceContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    [Function("CheckAbandonedCarts")]
    public async Task Run([TimerTrigger("0 * * * * *")] TimerInfo timer,FunctionContext context)
    {
        var logger = context.GetLogger("CheckAbandonedCarts");
        var cutoffDate = DateTime.UtcNow.AddDays(-3);

        var abandonedCarts = await _context.Cart
            .Include(c => c.CartItems)
            .Include(c => c.Users)
            .Where(c => c.CartItems.Any() && c.UpdatedAt <= cutoffDate && !c.NotifiedAbandoned)
            .ToListAsync();

        foreach (var cart in abandonedCarts)
        {
            await _notificationService.SendToUser(
                cart.UserId,
                "Items waiting in your cart",
                $"You have {cart.CartItems.Count} item(s) still in your cart. Complete your purchase before they're gone!",
                notificationTypeId: 1,
                referenceType: "Cart",
                referenceId: cart.CartId);

            cart.NotifiedAbandoned = true;
        }

        await _context.SaveChangesAsync();
        logger.LogInformation($"Checked abandoned carts, notified {abandonedCarts.Count} users.");
    }
}