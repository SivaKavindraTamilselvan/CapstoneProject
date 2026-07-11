using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
public class AdminDashboardService : IDashBoardService
{
    private readonly IOrderRepsository _orderRepository;

    public AdminDashboardService(IOrderRepsository orderRepsository)
    {
        _orderRepository = orderRepsository;
    }

    public async Task<decimal> GetGrossSalesAsync()
    {
        return await _orderRepository.GetGrossSalesAsync();
    }

    public async Task<decimal> GetCommissionAsync()
    {
        return await _orderRepository.GetCommissionAsync();
    }

    public async Task<decimal> GetYesterdayCommissionAsync()
    {
        return await _orderRepository.GetYesterdayCommissionAsync();
    }

    public async Task<decimal> GetLast30DaysCommissionAsync()
    {
        return await _orderRepository.GetLast30DaysCommissionAsync();
    }

    public async Task<int> GetTotalOrdersAsync()
    {
        return await _orderRepository.GetTotalOrdersAsync();
    }

    public async Task<int> GetPendingOrdersAsync()
    {
        return await _orderRepository.GetPendingOrdersAsync();
    }

    public async Task<int> GetTotalCustomersAsync()
    {
        return await _orderRepository.GetTotalCustomersAsync();
    }

    public async Task<int> GetActiveVendorsAsync()
    {
        return await _orderRepository.GetActiveVendorsAsync();
    }

    public async Task<List<RevenueTrendDto>> GetRevenueTrendAsync(string period)
    {
        return await _orderRepository.GetRevenueTrendAsync(period);
    }

    public async Task<List<ProductApprovalStatusDto>> GetProductApprovalStatusAsync()
    {
        return await _orderRepository.GetProductApprovalStatusAsync();
    }

    public async Task<List<ProductSubCategoryDto>> GetProductsPerSubCategoryAsync()
    {
        return await _orderRepository.GetProductsPerSubCategoryAsync();
    }
    public async Task<List<OrderStatusChartDto>> GetOrdersByStatusAsync()
    {
        return await _orderRepository.GetOrdersByStatusAsync();
    }

    public async Task<List<OrdersByMonthDto>> GetOrdersByMonthAsync()
    {
        return await _orderRepository.GetOrdersByMonthAsync();
    }
}