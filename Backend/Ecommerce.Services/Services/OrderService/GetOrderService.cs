using Ecommerce.DTOs;
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
    public async Task<PagedResponse<OrderSummaryDto>> GetOrderByAdmin(AdminOrderFilterParams orderFilterParams)
    {
        var order = await _orderRepsository.GetOrdersForAdmin(orderFilterParams);
        var result = _mapper.Map<List<OrderSummaryDto>>(order.items);

        foreach (var orderDto in result)
        {
            foreach (var orderItem in orderDto.OrderItems)
            {
                var image = await _productImageRepsository.GetMainImageByProduct(orderItem.ProductId);

                if (image != null)
                {
                    orderItem.ProductImageUrl = image.ImageUrl;
                }
            }
        }

        return new PagedResponse<OrderSummaryDto>
        {
            Items = result,
            PageNumber = orderFilterParams.PageNumber,
            PageSize = orderFilterParams.PageSize,
            TotalCount = order.totalCount
        };
    }
    // get order by user
    public async Task<PagedResponse<OrderSummaryDto>> GetOrderByUserId(OrderFilterParams orderFilterParams, int userid)
    {
        var user = await _userRepsository.Get(userid);
        if (user == null || !user.IsActive)
        {
            throw new DataNotFoundException("User Not Found");
        }

        var order = await _orderRepsository.GetOrdersForUser(userid, orderFilterParams);
        var result = _mapper.Map<List<OrderSummaryDto>>(order.items);

        var canReturnMap = new Dictionary<int, bool>();

        foreach (var item in order.items)
        {
            foreach (var orderItems in item.OrderItems)
            {
                var canReturn = false;

                if (orderItems.ProductVariant!.IsReturn && orderItems.OrderItemStatusId == 4)
                {
                    var shipment = await _shipmentRepsository.GetShipmentByOrderItemId(orderItems.OrderItemsId);

                    if (shipment?.DeliveryDate != null &&
                        DateTime.Now <= shipment.DeliveryDate.Value.AddDays(7))
                    {
                        var returnItem = await _returnRepsository.GetTheReturnProductByOrderItemId(orderItems.OrderItemsId);

                        if (returnItem == null || returnItem.ReturnStatusId != 1)
                        {
                            canReturn = true;
                        }
                    }
                }

                canReturnMap[orderItems.OrderItemsId] = canReturn;
            }
        }

        foreach (var orderDto in result)
        {
            foreach (var orderItem in orderDto.OrderItems)
            {
                var image = await _productImageRepsository.GetMainImageByProduct(orderItem.ProductId);

                if (image != null)
                {
                    orderItem.ProductImageUrl = image.ImageUrl;
                }

                orderItem.canReturn = canReturnMap.TryGetValue(orderItem.OrderItemsId, out var canReturn) && canReturn;
            }
        }

        return new PagedResponse<OrderSummaryDto>
        {
            Items = result,
            PageNumber = orderFilterParams.PageNumber,
            PageSize = orderFilterParams.PageSize,
            TotalCount = order.totalCount
        };
    }
    //user
    public async Task<OrderSummaryDto> GetOrderForUserByOrderId(int orderId)
    {
        var order = await _orderRepsository.GetOrderByOrderId(orderId);
        if (order == null || order.OrderId != orderId)
        {
            throw new DataNotFoundException("Order Not Found");
        }

        return _mapper.Map<OrderSummaryDto>(order);
    }
    //get order by vendor
    public async Task<PagedResponse<OrderItemSummaryDto>> GetOrderByVendor(OrderFilterParams orderFilterParams,
     int userId)
    {
        var user = await _vendorUserValidation.ValidateVendorUserByUserId(userId);

        if (user == null || !user.IsActive)
            throw new DataNotFoundException("User Not Found");

        var result = await _orderRepsository.GetOrderItemsForVendor(
       user.VendorId,
       orderFilterParams
   );


        return new PagedResponse<OrderItemSummaryDto>
        {
            Items = _mapper.Map<List<OrderItemSummaryDto>>(result.items),
            PageNumber = orderFilterParams.PageNumber,
            PageSize = orderFilterParams.PageSize,
            TotalCount = result.totalCount
        };
    }

    public async Task<OrderItemSummaryDto> GetOrderByOrderId(int orderId)
    {
        var result = await _orderRepsository.GetOrderItemsByOrderItemId(orderId);
        if (result == null)
        {
            Console.WriteLine("OrderItem is null");
        }
        else if (result.ProductVariant == null)
        {
            Console.WriteLine("ProductVariant is null");
        }
        else if (result.ProductVariant.Product == null)
        {
            Console.WriteLine("Product is null");
        }
        else
        {
            Console.WriteLine(result.ProductVariant.Product.ProductName);
        }
        return _mapper.Map<OrderItemSummaryDto>(result);
    }
}