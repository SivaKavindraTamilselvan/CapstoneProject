using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashBoardService _adminDashboardService;

    public DashboardController(IDashBoardService adminDashboardService)
    {
        _adminDashboardService = adminDashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> GetDashboard()
    {
        var dashboard = new
        {
            GrossSales = await _adminDashboardService.GetGrossSalesAsync(),
            Commission = await _adminDashboardService.GetCommissionAsync(),
            YesterdayCommission = await _adminDashboardService.GetYesterdayCommissionAsync(),
            Last30DaysCommission = await _adminDashboardService.GetLast30DaysCommissionAsync(),
            TotalOrders = await _adminDashboardService.GetTotalOrdersAsync(),
            PendingOrders = await _adminDashboardService.GetPendingOrdersAsync(),
            TotalCustomers = await _adminDashboardService.GetTotalCustomersAsync(),
            ActiveVendors = await _adminDashboardService.GetActiveVendorsAsync()
        };

        return Ok(dashboard);
    }

    [HttpGet("revenue-trend")]
    public async Task<IActionResult> GetRevenueTrend([FromQuery] string period = "7days")
    {
        var result = await _adminDashboardService.GetRevenueTrendAsync(period);

        return Ok(result);
    }
    [HttpGet("product-approval-status")]
    public async Task<IActionResult> GetProductApprovalStatus()
    {
        return Ok(await _adminDashboardService.GetProductApprovalStatusAsync());
    }

    [HttpGet("products-per-subcategory")]
    public async Task<IActionResult> GetProductsPerSubCategory()
    {
        return Ok(await _adminDashboardService.GetProductsPerSubCategoryAsync());
    }
    [HttpGet("orders-by-status")]
public async Task<IActionResult> GetOrdersByStatus()
{
    return Ok(await _adminDashboardService.GetOrdersByStatusAsync());
}

[HttpGet("orders-by-month")]
public async Task<IActionResult> GetOrdersByMonth()
{
    return Ok(await _adminDashboardService.GetOrdersByMonthAsync());
}
}