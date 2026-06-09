using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class OrderService : IOrderService
{
    public async Task<List<OrderSummaryDto>> GetOrderByAdmin(OrderFilterParams orderFilterParams)
    {
        var order = await _orderRepsository.GetOrdersForAdmin(orderFilterParams);
        return _mapper.Map<List<OrderSummaryDto>>(order);
    }
    public async Task<List<OrderSummaryDto>> GetOrderByUserId(OrderFilterParams orderFilterParams,int userid)
    {
        var user = await _userRepsository.Get(userid);
        if(user == null || !user.IsActive)
        {
            throw new DataNotFoundException("User Not Found");
        }
        var order = await _orderRepsository.GetOrdersForUser(userid,orderFilterParams);
        return _mapper.Map<List<OrderSummaryDto>>(order);
    }
    public async Task<List<OrderSummaryDto>> GetOrderByVendor(OrderFilterParams orderFilterParams,int userid)
    {
        var user = await _vendorUserValidation.ValidateVendorUserByUserId(userid);
        if(!user.Vendor!.IsActive)
        {
            throw new DataNotFoundException("Vendor Not Found");
        }
        if(user == null || !user.IsActive)
        {
            throw new DataNotFoundException("User Not Found");
        }
        var order = await _orderRepsository.GetOrdersForVendor(user.VendorId,orderFilterParams);
        return _mapper.Map<List<OrderSummaryDto>>(order);
    }
}