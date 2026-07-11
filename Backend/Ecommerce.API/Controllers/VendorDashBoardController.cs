using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class VendorDashboardController : ControllerBase
{
    private readonly IVendorDashboardService _vendorDashboardService;

    public VendorDashboardController(IVendorDashboardService vendorDashboardService)
    {
        _vendorDashboardService = vendorDashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);


        var dashboard = new
        {
            GrossSales = await _vendorDashboardService.GetVendorGrossSalesAsync(vendorId),
            YesterdaySales = await _vendorDashboardService.GetVendorYesterdaySalesAsync(vendorId),
            Last30DaysSales = await _vendorDashboardService.GetVendorLast30DaysSalesAsync(vendorId),
            TotalOrders = await _vendorDashboardService.GetVendorTotalOrdersAsync(vendorId),
            PendingOrders = await _vendorDashboardService.GetVendorPendingOrdersAsync(vendorId),
            TotalProducts = await _vendorDashboardService.GetVendorTotalProductsAsync(vendorId),
            ActiveProducts = await _vendorDashboardService.GetVendorActiveProductsAsync(vendorId),
            LowStockProducts = await _vendorDashboardService.GetVendorLowStockProductsAsync(vendorId)
        };

        return Ok(dashboard);
    }

    [HttpGet("revenue-trend")]
    public async Task<IActionResult> GetRevenueTrend([FromQuery] string period = "7days")
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return Ok(await _vendorDashboardService.GetVendorRevenueTrendAsync(vendorId, period));
    }
    [HttpGet("product-approval-status")]
    public async Task<IActionResult> GetProductApprovalStatus()
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(await _vendorDashboardService.GetVendorProductApprovalStatusAsync(vendorId));
    }

    [HttpGet("products-per-subcategory")]
    public async Task<IActionResult> GetProductsPerSubCategory()
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(await _vendorDashboardService.GetVendorProductsPerSubCategoryAsync(vendorId));
    }

    [HttpGet("orders-by-status")]
    public async Task<IActionResult> GetOrdersByStatus()
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(await _vendorDashboardService.GetVendorOrdersByStatusAsync(vendorId));
    }

    [HttpGet("orders-by-month")]
    public async Task<IActionResult> GetOrdersByMonth()
    {
        int vendorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        return Ok(await _vendorDashboardService.GetVendorOrdersByMonthAsync(vendorId));
    }
}