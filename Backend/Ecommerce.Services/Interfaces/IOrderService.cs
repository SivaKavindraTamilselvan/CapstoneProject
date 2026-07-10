using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IOrderService
{
    public Task<OrderSummaryDto> GetOrderForAdminByOrderId(int orderId);
    public Task<List<OrderSummaryDto>> GetOrderByAdmin(AdminOrderFilterParams orderFilterParams);

    public Task<List<OrderItemSummaryDto>> GetOrderByVendor(OrderFilterParams orderFilterParams, int userid);

    public Task<OrderSummaryDto> GetOrderForUserByOrderId(int orderId);
    public Task<List<OrderSummaryDto>> GetOrderByUserId(OrderFilterParams orderFilterParams, int userid);
    
    public Task ConfirmOrderStatus(int orderId,bool status);

    public Task<Order> CreateOrder(RequestCreateOrderDTO requestCreateOrderDTO);
    public Task<List<OrderItems>> CreateOrderItems(List<SelectedCartInventory> selectedItems, Order order, Coupons? selectedCoupon);
}