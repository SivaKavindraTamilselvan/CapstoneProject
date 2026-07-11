
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorDashboardService : IVendorDashboardService
{
    private readonly IOrderRepsository _orderRepository;
    private readonly IVendorUserValidation _vendorUserValidation;

    public VendorDashboardService(IVendorUserValidation vendorUserValidation, IOrderRepsository orderRepository)
    {
        _vendorUserValidation = vendorUserValidation;
        _orderRepository = orderRepository;
    }

    public async Task<decimal> GetVendorGrossSalesAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorGrossSalesAsync(vendor.VendorId);
    }

    public async Task<decimal> GetVendorYesterdaySalesAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorYesterdaySalesAsync(vendor.VendorId);
    }

    public async Task<decimal> GetVendorLast30DaysSalesAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorLast30DaysSalesAsync(vendor.VendorId);
    }

    public async Task<int> GetVendorTotalOrdersAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorTotalOrdersAsync(vendor.VendorId);
    }

    public async Task<int> GetVendorPendingOrdersAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorPendingOrdersAsync(vendor.VendorId);
    }

    public async Task<int> GetVendorTotalProductsAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorTotalProductsAsync(vendor.VendorId);
    }

    public async Task<int> GetVendorActiveProductsAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorActiveProductsAsync(vendor.VendorId);
    }

    public async Task<int> GetVendorLowStockProductsAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorLowStockProductsAsync(vendor.VendorId);
    }

    public async Task<List<RevenueTrendDto>> GetVendorRevenueTrendAsync(int vendorId, string period)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorRevenueTrendAsync(vendor.VendorId, period);
    }

    public async Task<List<ProductApprovalStatusDto>> GetVendorProductApprovalStatusAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorProductApprovalStatusAsync(vendor.VendorId);
    }

    public async Task<List<ProductSubCategoryDto>> GetVendorProductsPerSubCategoryAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorProductsPerSubCategoryAsync(vendor.VendorId);
    }

    public async Task<List<OrderStatusChartDto>> GetVendorOrdersByStatusAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorOrdersByStatusAsync(vendor.VendorId);
    }

    public async Task<List<OrdersByMonthDto>> GetVendorOrdersByMonthAsync(int vendorId)
    {
        var vendor = await _vendorUserValidation.ValidateVendorUserByUserId(vendorId);
        return await _orderRepository.GetVendorOrdersByMonthAsync(vendor.VendorId);
    }
}