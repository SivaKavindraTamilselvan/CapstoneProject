using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserOrderService : IUserOrderService
{
    private async Task<Address> ValidateAddress(int addressId, int userid)
    {
        _logger.LogInformation("Validating AddressId {AddressId} for UserId {UserId}", addressId, userid);

        var address = await _addressRepsository.Get(addressId);
        if (address == null)
        {
            _logger.LogWarning("AddressId {AddressId} not found", addressId);
            throw new DataNotFoundException("Address Not Found");
        }
        if (address.UserId != userid)
        {
            _logger.LogWarning("AddressId {AddressId} does not belong to UserId {UserId}. Actual owner UserId {OwnerUserId}", addressId, userid, address.UserId);
            throw new DataApprovalStatusException("Address not mapped to the correct user");
        }

        _logger.LogInformation("AddressId {AddressId} validated successfully for UserId {UserId}", addressId, userid);
        return address;
    }

    private async Task<Coupons> ValidateCoupon(int userId, int couponId)
    {
        _logger.LogInformation("Validating CouponId {CouponId} for UserId {UserId}", couponId, userId);

        var validateCoupon = await _userCouponService.GetAllAvailableCoupons(userId);
        Coupons? selectedCoupon = null;

        selectedCoupon = validateCoupon.FirstOrDefault(c => c.CouponId == couponId);
        if (selectedCoupon == null)
        {
            _logger.LogWarning("CouponId {CouponId} is not available for UserId {UserId}", couponId, userId);
            throw new DataNotFoundException("The Coupon Is Not Available For You");
        }

        _logger.LogInformation("CouponId {CouponId} validated successfully for UserId {UserId}. DiscountValue {DiscountValue}", couponId, userId, selectedCoupon.DiscountValue);
        return selectedCoupon;
    }

    private async Task<List<CartItems>> ValidateProduct(int userId)
    {
        _logger.LogInformation("Validating cart items for UserId {UserId}", userId);

        var cartItems = await _cartItemsRepsository.GetCartItemsByUserId(userId);
        if (cartItems.Count == 0)
        {
            _logger.LogWarning("No cart items found for UserId {UserId}", userId);
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        if (cartItems.Any(c => c.ProductVariant == null))
        {
            _logger.LogWarning("One or more cart items have no ProductVariant loaded for UserId {UserId}", userId);
            throw new DataNotFoundException("Product variant not loaded");
        }
        _logger.LogInformation("Found {ItemCount} cart items for UserId {UserId}. Checking availability", cartItems.Count, userId);

        foreach (var items in cartItems)
        {
            var product = await _productRepsository.CheckTheProduct(items.ProductVariantId, items.Qunatity);
            if (product == null)
            {
                _logger.LogWarning("ProductVariantId {ProductVariantId} (SKU {SKU}) not available for requested Quantity {Quantity} for UserId {UserId}", items.ProductVariantId, items.ProductVariant!.SKU, items.Qunatity, userId);
                throw new DataApprovalStatusException($"Order Cannot Be Placed As {items.ProductVariant!.SKU} Is not available currently");
            }
        }

        _logger.LogInformation("All {ItemCount} cart items validated and available for UserId {UserId}", cartItems.Count, userId);
        return cartItems;
    }
}