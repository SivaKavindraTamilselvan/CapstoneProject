using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IOrderService
{
    public Task ConfirmOrderStatus(int orderId);
    public Task<Order> CreateOrder(RequestCreateOrderDTO requestCreateOrderDTO);
    public Task<List<OrderItems>> CreateOrderItems(List<SelectedCartInventory> selectedItems, Order order, Coupons? selectedCoupon);
}