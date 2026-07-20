using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

public partial class UserOrderService : IUserOrderService
{
    // add order if the cart is pressed to checkout
    public async Task<ShippingCheckResponseDTO> CheckService(RequestAddOrderDTO requestAddOrderDTO,int userId)
    {
        _logger.LogInformation("UserId {UserId} checking shipping serviceability for AddressId {AddressId}", userId, requestAddOrderDTO.AddressId);

        var cartItems = await ValidateProduct(userId);
        _logger.LogInformation("Cart items validated for UserId {UserId}. ItemCount {ItemCount}", userId, cartItems.Count);

        var deliveryAddress = await ValidateAddress(requestAddOrderDTO.AddressId, userId);
        _logger.LogInformation("Delivery AddressId {AddressId} validated for UserId {UserId}", deliveryAddress.AddressId, userId);

        decimal productCharge = CalculateProductCharge(cartItems);

        int cod = requestAddOrderDTO.PaymentMethod == 1 ? 1 : 0;

        Coupons? selectedCoupon = null;

        if (requestAddOrderDTO.CouponId != null)
        {
            selectedCoupon = await ValidateCoupon(userId, requestAddOrderDTO.CouponId.Value);
            _logger.LogInformation("CouponId {CouponId} validated for UserId {UserId}. DiscountValue {DiscountValue}", requestAddOrderDTO.CouponId, userId, selectedCoupon?.DiscountValue);
        }

        decimal couponCharge = selectedCoupon?.DiscountValue ?? 0;

        var selectedItems = await GetTheInventoryPickupAddress(cartItems, deliveryAddress, cod);
        _logger.LogInformation("Resolved inventory pickup addresses for {ItemCount} selected items", selectedItems.Count);

        var groupedShipments = selectedItems
            .GroupBy(x => new
            {
                VendorId = x.CartItem.ProductVariant!.Product!.VendorId,
                PickupAddressId = x.Inventory.AddressId
            });
        _logger.LogInformation("Grouped selected items into {ShipmentGroupCount} shipment groups", groupedShipments.Count());

        decimal totalShippingCharge = 0;

        foreach (var group in groupedShipments)
        {
            var pickupAddress = group.First().Inventory.Address!;

            decimal shipmentWeight = group.Sum(x =>
                x.CartItem.Qunatity * x.CartItem.ProductVariant!.WeightInKgs);

            _logger.LogInformation("Checking shipping charge for PickupAddressId {PickupAddressId}, VendorId {VendorId}, Weight {Weight}kg", group.Key.PickupAddressId, group.Key.VendorId, shipmentWeight);

            decimal shippingCharge = await CalculateShippingCharge(
                pickupAddress,
                deliveryAddress,
                shipmentWeight,
                cod);

            totalShippingCharge += shippingCharge;
            _logger.LogInformation("ShippingCharge {ShippingCharge} calculated for PickupAddressId {PickupAddressId}. Running TotalShippingCharge {TotalShippingCharge}", shippingCharge, group.Key.PickupAddressId, totalShippingCharge);
        }

        _logger.LogInformation("Shipping serviceability check completed for UserId {UserId}. TotalShippingCharge {TotalShippingCharge}", userId, totalShippingCharge);

        return new ShippingCheckResponseDTO
        {
            TotalShippingCharge = totalShippingCharge,
            IsShippingAvailable = true
        };
    }
}