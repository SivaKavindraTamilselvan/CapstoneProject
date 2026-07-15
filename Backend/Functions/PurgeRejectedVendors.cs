using Ecommerce.Data;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Functions;

public class PurgeRejectedVendors
{
    private readonly EcommerceContext _context;

    public PurgeRejectedVendors(EcommerceContext context)
    {
        _context = context;
    }

    [Function("PurgeRejectedVendors")]
    public async Task Run(
        [TimerTrigger("0 0 2 * * *")] TimerInfo timer,
        FunctionContext context)
    {
        var logger = context.GetLogger("PurgeRejectedVendors");
        var cutoffDate = DateTime.UtcNow.AddDays(-30);

        var vendorsToDelete = await _context.Vendor
            .Include(v => v.VendorUsers).ThenInclude(u => u.User)
            .Where(v => v.ApprovalStatusId == 3
                        && v.ReviewedAt != null
                        && v.ReviewedAt <= cutoffDate)
            .ToListAsync();

        if (!vendorsToDelete.Any())
        {
            logger.LogInformation("No rejected vendors older than 30 days found.");
            return;
        }

        foreach (var vendor in vendorsToDelete)
        {
            foreach (var vendorUser in vendor.VendorUsers)
            {
                var user = vendorUser.User;
                _context.VendorUser.Remove(vendorUser);

                if (user != null)
                {
                    _context.User.Remove(user);
                }
            }

            // Remove the vendor once, outside the inner loop
            _context.Vendor.Remove(vendor);
        }

        await _context.SaveChangesAsync();

        logger.LogInformation(
            $"Purged {vendorsToDelete.Count} rejected vendor(s) and their linked user records.");
    }
}