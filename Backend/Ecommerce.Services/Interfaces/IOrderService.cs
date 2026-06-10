using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IOrderService
{
    public Task<OrderSummaryDto> GetOrderByAdminId(int orderId);
    public Task<List<OrderSummaryDto>> GetOrderByVendor(OrderFilterParams orderFilterParams, int userid);
    public Task<List<OrderSummaryDto>> GetOrderByUserId(OrderFilterParams orderFilterParams, int userid);
    public Task<List<OrderSummaryDto>> GetOrderByAdmin(OrderFilterParams orderFilterParams);
    public Task ConfirmOrderStatus(int orderId,bool status);
    public Task<Order> CreateOrder(RequestCreateOrderDTO requestCreateOrderDTO);
    public Task<List<OrderItems>> CreateOrderItems(List<SelectedCartInventory> selectedItems, Order order, Coupons? selectedCoupon);
}