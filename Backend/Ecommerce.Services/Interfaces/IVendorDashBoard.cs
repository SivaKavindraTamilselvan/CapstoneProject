public interface IVendorDashboardService
{
    Task<decimal> GetVendorGrossSalesAsync(int vendorId);
    Task<decimal> GetVendorYesterdaySalesAsync(int vendorId);
    Task<decimal> GetVendorLast30DaysSalesAsync(int vendorId);

    Task<int> GetVendorTotalOrdersAsync(int vendorId);
    Task<int> GetVendorPendingOrdersAsync(int vendorId);

    Task<int> GetVendorTotalProductsAsync(int vendorId);
    Task<int> GetVendorActiveProductsAsync(int vendorId);
    Task<int> GetVendorLowStockProductsAsync(int vendorId);

    Task<List<RevenueTrendDto>> GetVendorRevenueTrendAsync(int vendorId, string period);

    Task<List<ProductApprovalStatusDto>> GetVendorProductApprovalStatusAsync(int vendorId);
    Task<List<ProductSubCategoryDto>> GetVendorProductsPerSubCategoryAsync(int vendorId);

    Task<List<OrderStatusChartDto>> GetVendorOrdersByStatusAsync(int vendorId);
    Task<List<OrdersByMonthDto>> GetVendorOrdersByMonthAsync(int vendorId);
}