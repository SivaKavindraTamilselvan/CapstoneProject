using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public partial class OrderService : IOrderService
{
    //create order 
    // linked with the cart
    public async Task<Order> CreateOrder(RequestCreateOrderDTO requestCreateOrderDTO)
    {
        var order = _mapper.Map<Order>(requestCreateOrderDTO);
        order.OrderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
        order.FinalAmount = order.TotalProductAmount - order.TotalCouponAmount + order.TotalShippingAmount;
        order.OrderStatusId = 1;
        order.OrderDate = DateTime.Now;
        await _orderRepsository.Create(order);
        return order;
    }
    //create order 
    // linked with the cart
    public async Task<List<OrderItems>> CreateOrderItems(List<SelectedCartInventory> selectedItems, Order order, Coupons? selectedCoupon)
    {
        List<OrderItems> orderItemsList = new List<OrderItems>();

        foreach (var selected in selectedItems)
        {
            var cartItem = selected.CartItem;

            OrderItems orderItems = new OrderItems();
            orderItems.ProductVariantId = cartItem.ProductVariantId;
            orderItems.InventoryId = selected.Inventory.InventoryId;
            orderItems.Quantity = cartItem.Qunatity;
            orderItems.OrderId = order.OrderId;
            orderItems.UnitPrice = cartItem.ProductVariant!.Price;
            orderItems.Discount = 0;
            orderItems.OrderItemStatusId = 1;

            if (selectedCoupon != null)
            {
                bool applicable = selectedCoupon.CouponsProducts.Any(p => p.ProductId == cartItem.ProductVariant!.ProductId);
                if (applicable)
                {
                    orderItems.Discount = selectedCoupon.DiscountValue;
                }
            }
            var createdOrderItem = await _orderItemRepsository.Create(orderItems);
            if (createdOrderItem != null)
            {
                orderItemsList.Add(createdOrderItem);
            }
            var inventory = await _inventoryValidation.ValidateInventory(selected.Inventory.InventoryId);
            inventory.AvailableQuantity = inventory.AvailableQuantity - cartItem.Qunatity;
            inventory.ReservedQuantity = inventory.ReservedQuantity + cartItem.Qunatity;
            await _inventoryRepsository.Update(inventory.InventoryId, inventory);
            if (selectedCoupon != null)
            {
                await CreateCouponUsage(order.OrderId, selectedCoupon.CouponId);
            }
        }

        return orderItemsList;
    }
    private async Task CreateCouponUsage(int orderId, int couponId)
    {
        CouponUsage couponUsage = new CouponUsage();
        couponUsage.CouponId = couponId;
        couponUsage.OrderId = orderId;
        await _couponUsageRepsository.Create(couponUsage);
    }
}