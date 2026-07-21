using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IOrderService
{
        public  Task<OrderInvoiceDto> GetOrderInvoiceData(int orderId);
        public  Task<OrderItemSummaryDto> GetOrderByOrderId(int orderId);
    public Task<OrderSummaryDto> GetOrderForAdminByOrderId(int orderId);
    public Task<PagedResponse<OrderSummaryDto>> GetOrderByAdmin(AdminOrderFilterParams orderFilterParams);

    public Task<PagedResponse<OrderItemSummaryDto>> GetOrderByVendor(OrderFilterParams orderFilterParams, int userid);

    public Task<OrderSummaryDto> GetOrderForUserByOrderId(int orderId);
    public Task<PagedResponse<OrderSummaryDto>> GetOrderByUserId(OrderFilterParams orderFilterParams, int userid);
    
    public Task ConfirmOrderStatus(int orderId,bool status);

    public Task<Order> CreateOrder(RequestCreateOrderDTO requestCreateOrderDTO);
    public Task<List<OrderItems>> CreateOrderItems(List<SelectedCartInventory> selectedItems, Order order, Coupons? selectedCoupon);
}