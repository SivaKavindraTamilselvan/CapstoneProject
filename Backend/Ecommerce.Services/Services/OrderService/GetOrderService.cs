using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class OrderService : IOrderService
{
    //admin 
    public async Task<OrderSummaryDto> GetOrderForAdminByOrderId(int orderId)
    {
        var order = await _orderRepsository.GetOrderByOrderId(orderId);
        return _mapper.Map<OrderSummaryDto>(order);
    }

    //admin get all order
    public async Task<List<OrderSummaryDto>> GetOrderByAdmin(AdminOrderFilterParams orderFilterParams)
    {
        var orderfilter = _mapper.Map<OrderFilterParams>(orderFilterParams);
        var order = await _orderRepsository.GetOrdersForAdmin(orderFilterParams, orderfilter);
        return _mapper.Map<List<OrderSummaryDto>>(order);
    }
    // get order by user
    public async Task<List<OrderSummaryDto>> GetOrderByUserId(OrderFilterParams orderFilterParams, int userid)
    {
        var user = await _userRepsository.Get(userid);
        if (user == null || !user.IsActive)
        {
            throw new DataNotFoundException("User Not Found");
        }
        var order = await _orderRepsository.GetOrdersForUser(userid, orderFilterParams);
        return _mapper.Map<List<OrderSummaryDto>>(order);
    }
    //user
    public async Task<OrderSummaryDto> GetOrderForUserByOrderId(int orderId)
    {
        var order = await _orderRepsository.GetOrderByOrderId(orderId);
        if (order == null || order.UserId != orderId)
        {
            throw new DataNotFoundException("Order Not Found");
        }
        return _mapper.Map<OrderSummaryDto>(order);
    }
    //get order by vendor
    public async Task<List<OrderItemSummaryDto>> GetOrderByVendor(OrderFilterParams orderFilterParams, int userid)
    {
        var user = await _vendorUserValidation.ValidateVendorUserByUserId(userid);
        if (user == null || !user.IsActive)
        {
            throw new DataNotFoundException("User Not Found");
        }
        var order = await _orderRepsository.GetOrdersForVendor(user.VendorId, orderFilterParams);
        var orderItems = order
        .Where(o => o.OrderStatusId == 2)
         .SelectMany(o => o.OrderItems)
         .Where(oi => oi.ProductVariant != null &&
                     oi.ProductVariant.Product != null &&
                     oi.ProductVariant.Product.VendorId == user.VendorId)
         .ToList();
        return _mapper.Map<List<OrderItemSummaryDto>>(orderItems);
    }
}