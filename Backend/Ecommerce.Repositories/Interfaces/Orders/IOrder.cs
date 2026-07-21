using Ecommerce.Models;

namespace Ecommerce.Repositories.Interfaces;

public interface IOrderRepsository : IRepository<int, Order>
{
    public  Task<int> GetNumberOfOrderItems(int orderId);
    public Task<decimal> GetVendorGrossSalesAsync(int vendorId);
    public Task<decimal> GetVendorYesterdaySalesAsync(int vendorId);
    public Task<decimal> GetVendorLast30DaysSalesAsync(int vendorId);
    public Task<int> GetVendorTotalOrdersAsync(int vendorId);
    public Task<int> GetVendorPendingOrdersAsync(int vendorId);
    public Task<int> GetVendorTotalProductsAsync(int vendorId);
    public Task<int> GetVendorActiveProductsAsync(int vendorId);
    public Task<int> GetVendorLowStockProductsAsync(int vendorId);
    public Task<List<RevenueTrendDto>> GetVendorRevenueTrendAsync(int vendorId, string period);
    public Task<List<ProductApprovalStatusDto>> GetVendorProductApprovalStatusAsync(int vendorId);
    public Task<List<OrderStatusChartDto>> GetVendorOrdersByStatusAsync(int vendorId);
    public Task<List<OrdersByMonthDto>> GetVendorOrdersByMonthAsync(int vendorId);
    public Task<List<ProductSubCategoryDto>> GetVendorProductsPerSubCategoryAsync(int vendorId);


    public Task<OrderItems?> GetOrderItemsByOrderItemId(int orderItemId);
    Task<List<OrderStatusChartDto>> GetOrdersByStatusAsync();
    Task<List<OrdersByMonthDto>> GetOrdersByMonthAsync();
    Task<List<ProductApprovalStatusDto>> GetProductApprovalStatusAsync();
    Task<List<ProductSubCategoryDto>> GetProductsPerSubCategoryAsync();
    public Task<List<RevenueTrendDto>> GetRevenueTrendAsync(string period);
    Task<decimal> GetGrossSalesAsync();
    Task<decimal> GetCommissionAsync();
    Task<decimal> GetYesterdayCommissionAsync();
    Task<decimal> GetLast30DaysCommissionAsync();
    Task<int> GetTotalOrdersAsync();
    Task<int> GetPendingOrdersAsync();
    Task<int> GetTotalCustomersAsync();
    Task<int> GetActiveVendorsAsync();
    public Task<Order?> GetOrderByOrderId(int orderId);
    public Task<(List<Order> items, int totalCount)> GetOrdersForUser(int userId, OrderFilterParams filters);
    public Task<(List<Order> items, int totalCount)> GetOrdersForAdmin(AdminOrderFilterParams filters);
    public Task<(List<OrderItems> items, int totalCount)> GetOrderItemsForVendor(int vendorId, OrderFilterParams filters);
    public Task<List<Order>> GetPendingOrdersByAddress(int address);
}