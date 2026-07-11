
namespace Ecommerce.Services.Interfaces
{
    public interface IDashBoardService
    {
        Task<List<OrderStatusChartDto>> GetOrdersByStatusAsync();
Task<List<OrdersByMonthDto>> GetOrdersByMonthAsync();
        Task<List<ProductApprovalStatusDto>> GetProductApprovalStatusAsync();
Task<List<ProductSubCategoryDto>> GetProductsPerSubCategoryAsync();
        Task<decimal> GetGrossSalesAsync();
        Task<decimal> GetCommissionAsync();
        Task<decimal> GetYesterdayCommissionAsync();
        Task<decimal> GetLast30DaysCommissionAsync();
        Task<List<RevenueTrendDto>> GetRevenueTrendAsync(string period);

        Task<int> GetTotalOrdersAsync();
        Task<int> GetPendingOrdersAsync();
        Task<int> GetTotalCustomersAsync();
        Task<int> GetActiveVendorsAsync();
    }
}