using Ecommerce.Models;
using Ecommerce.Models.Exceptions;
using Ecommerce.Services.Interfaces;

public partial class UserOrderService : IUserOrderService
{
    private async Task<Address> ValidateAddress(int addressId,int userid)
    {
        var address = await _addressRepsository.Get(addressId);
        if (address == null)
        {
            throw new DataNotFoundException("Address Not Found");
        }
        if(address.UserId != userid)
        {
            throw new DataApprovalStatusException("Address not mapped to the correct user");
        }
        return address;
    }
    private async Task<Coupons> ValidateCoupon(int userId, int couponId)
    {
        var validateCoupon = await _userCouponService.GetAllAvailableCoupons(userId);
        Coupons? selectedCoupon = null;

        selectedCoupon = validateCoupon.FirstOrDefault(c => c.CouponId == couponId);
        if (selectedCoupon == null)
        {
            throw new DataNotFoundException("The Coupon Is Not Available For You");
        }
        return selectedCoupon;

    }
    private async Task<List<CartItems>> ValidateProduct(int userId)
    {
        var cartItems = await _cartItemsRepsository.GetCartItemsByUserId(userId);
        if (cartItems.Count == 0)
        {
            throw new DataNotFoundException("Cart Items Is Not Found");
        }
        if (cartItems.Any(c => c.ProductVariant == null))
        {
            throw new DataNotFoundException("Product variant not loaded");
        }
        foreach(var items in cartItems)
        {
            var product = await _productRepsository.CheckTheProduct(items.ProductVariantId,items.Qunatity);
            if(product == null)
            {
                throw new DataApprovalStatusException($"Order Cannot Be Placed As {items.ProductVariant!.SKU} Is not available currently");
            }
        }
        return cartItems;
    }
}