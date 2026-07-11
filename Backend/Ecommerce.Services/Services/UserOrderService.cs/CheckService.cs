using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

public partial class UserOrderService : IUserOrderService
{
    // add order if the cart is pressed to checkout
    public async Task<ShippingCheckResponseDTO> CheckService(
    RequestAddOrderDTO requestAddOrderDTO,
    int userId)
    {
        var cartItems = await ValidateProduct(userId);

        var deliveryAddress = await ValidateAddress(requestAddOrderDTO.AddressId, userId);

        decimal productCharge = CalculateProductCharge(cartItems);

        int cod = requestAddOrderDTO.PaymentMethod == 1 ? 1 : 0;

        Coupons? selectedCoupon = null;

        if (requestAddOrderDTO.CouponId != null)
        {
            selectedCoupon = await ValidateCoupon(userId, requestAddOrderDTO.CouponId.Value);
        }

        decimal couponCharge = selectedCoupon?.DiscountValue ?? 0;

        var selectedItems = await GetTheInventoryPickupAddress(cartItems, deliveryAddress, cod);

        var groupedShipments = selectedItems
            .GroupBy(x => new
            {
                VendorId = x.CartItem.ProductVariant!.Product!.VendorId,
                PickupAddressId = x.Inventory.AddressId
            });

        decimal totalShippingCharge = 0;

        foreach (var group in groupedShipments)
        {
            var pickupAddress = group.First().Inventory.Address!;

            decimal shipmentWeight = group.Sum(x =>
                x.CartItem.Qunatity * x.CartItem.ProductVariant!.WeightInKgs);

            decimal shippingCharge = await CalculateShippingCharge(
                pickupAddress,
                deliveryAddress,
                shipmentWeight,
                cod);

            totalShippingCharge += shippingCharge;
        }

        return new ShippingCheckResponseDTO
        {
            TotalShippingCharge = totalShippingCharge,
            IsShippingAvailable = true
        };
    }
}