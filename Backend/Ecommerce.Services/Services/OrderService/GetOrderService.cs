using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class OrderService : IOrderService
{
    //admin 
    public async Task<OrderSummaryDto> GetOrderForAdminByOrderId(int orderId)
    {
        var order = await _orderRepsository.GetOrderByOrderId(orderId);
        if (order == null || order.OrderId != orderId)
        {
            throw new DataNotFoundException("Order Not Found");
        }

        var dto = _mapper.Map<OrderSummaryDto>(order);

        var successfulPayment = order.Payments?
            .FirstOrDefault(p => p.PaymentStatus.PaymentStatusName == "Success");

        if (successfulPayment != null)
        {
            dto.PaymentStatus = "Success";
            dto.PaymentType = successfulPayment.ModeOfPayment.ModeOfPaymentName ?? string.Empty;
        }
        else
        {
            dto.PaymentStatus = "Failed";
            var lastAttempt = order.Payments?.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
            dto.PaymentType = lastAttempt?.ModeOfPayment?.ModeOfPaymentName ?? string.Empty;
        }

        return dto;
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

        for (int i = 0; i < order.items.Count; i++)
        {
            var sourceOrder = order.items[i];
            var orderDto = result[i];

            var successfulPayment = sourceOrder.Payments?
            .FirstOrDefault(p => p.PaymentStatus.PaymentStatusName == "Success" || p.PaymentStatusId == /* your success status id */ 2);

            if (successfulPayment != null)
            {
                orderDto.PaymentStatus = "Success";
                orderDto.PaymentType = successfulPayment.ModeOfPayment.ModeOfPaymentName ?? successfulPayment.ModeOfPayment.ModeOfPaymentName ?? string.Empty;
            }
            else
            {
                orderDto.PaymentStatus = "Failed";
                var lastAttempt = sourceOrder.Payments?.OrderByDescending(p => p.CreatedAt).FirstOrDefault();
                orderDto.PaymentType = lastAttempt?.ModeOfPayment.ModeOfPaymentName ?? lastAttempt?.ModeOfPayment.ModeOfPaymentName ?? string.Empty;
            }

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
            _logger.LogWarning("OrderItemId {OrderId} not found", orderId);
            throw new DataNotFoundException("Order item not found");
        }

        var overallOrder = await _orderRepsository.Get(result.OrderId);
        if (overallOrder == null)
        {
            _logger.LogWarning("OrderId {OrderId} not found", result.OrderId);
            throw new DataNotFoundException("Order not found");
        }

        var order = _mapper.Map<OrderItemSummaryDto>(result);
        if (order == null)
        {
            _logger.LogError("Mapping failed for OrderItemId {OrderId}", orderId);
            throw new NullReferenceException("Order mapping failed");
        }

        // Shipment may legitimately be missing if the order item was cancelled/returned
        var shipment = await _shipmentRepsository.GetShipmentByOrderItemId(orderId);
        if (shipment == null)
        {
            _logger.LogInformation("No shipment found for OrderItemId {OrderId} (likely cancelled/returned)", orderId);
        }

        int shipmentItems = 0;
        if (shipment != null)
        {
            shipmentItems = await _shipmentRepsository.GetShipmentItemNumber(shipment.ShipmentId);
            if (shipmentItems == 0)
            {
                _logger.LogInformation("No shipment items found for ShipmentId {ShipmentId}", shipment.ShipmentId);
            }
        }

        var orderItems = await _orderRepsository.GetNumberOfOrderItems(result.OrderId);
        if (orderItems == 0)
        {
            _logger.LogInformation("No order items found for OrderId {OrderId} (likely cancelled/returned)", result.OrderId);
        }

        // Safe division: default to 0 when denominators are missing/zero
        order.ShippingCharge = (shipment != null && shipmentItems > 0)
            ? shipment.ShippingCharge / shipmentItems
            : 0;

        order.Coupon = orderItems > 0 ? overallOrder.TotalCouponAmount / orderItems : 0;
        order.Wallet = orderItems > 0 ? overallOrder.TotalWalletAmount / orderItems : 0;

        order.OverallCost = order.UnitPrice * order.Quantity + order.ShippingCharge - order.Coupon - order.Wallet;
        if(order.OverallCost < 0)
        {
            order.OverallCost = 0;
        }

        return order;
    }
    public async Task<OrderInvoiceDto> GetOrderInvoiceData(int orderId)
    {
        var order = await _orderRepsository.GetOrderByOrderId(orderId);
        if (order == null)
        {
            throw new DataNotFoundException("Order not found");
        }
        var dto = _mapper.Map<OrderInvoiceDto>(order);
        var successfulPayment = order.Payments?
            .FirstOrDefault(p => p.PaymentStatus.PaymentStatusName == "Success");

        if (successfulPayment != null)
        {
            dto.PaymentMethod = successfulPayment.ModeOfPayment.ModeOfPaymentName ?? string.Empty;
        }
        else
        {
            dto.PaymentMethod = "Cash On Delivery";
        }
        return dto;

    }
}