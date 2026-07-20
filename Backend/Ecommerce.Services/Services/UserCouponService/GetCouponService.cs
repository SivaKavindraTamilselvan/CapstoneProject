using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Services.Interfaces;
using Microsoft.Extensions.Logging;

public partial class UserCouponService : IUserCouponService
{
    public async Task<List<ResponseGetAllCoupon>> GetAllActiveCoupons(int userId)
    {
        await _userValidation.ValidateUser(userId);
        _logger.LogInformation("Getting all active coupons for UserId: {UserId}", userId);
        var coupons = await _couponValidation.ValidateGetAllActiveCoupon(userId);
        _logger.LogInformation("Retrieved {CouponCount} active coupons for UserId: {UserId}", coupons.Count, userId);
        return _mapper.Map<List<ResponseGetAllCoupon>>(coupons);
    }

    public async Task<List<Coupons>> GetAllAvailableCoupons(int userId)
    {
        await _userValidation.ValidateUser(userId);
        _logger.LogInformation("Getting available coupons for UserId: {UserId}", userId);
        var cart = await _cartValidation.ValidateGetCartItemsByUserId(userId);
        var cost = cart.Sum(c => c.Qunatity * c.ProductVariant!.Price);
        var productId = cart.Select(c => c.ProductVariant!.ProductId).Distinct().ToList();
        _logger.LogInformation("Calculated cart total: {CartTotal} with {ProductCount} distinct products for UserId: {UserId}", cost, productId.Count, userId);
        var coupons = await _couponValidation.ValidateGetAllAvailableCoupons(cost, productId, userId);
        _logger.LogInformation("Retrieved {CouponCount} available coupons for UserId: {UserId}", coupons.Count, userId);
        return coupons;
    }

    public async Task<List<ResponseGetAllCoupon>> GetAllAvailableCouponsUser(int userId)
    {
        await _userValidation.ValidateUser(userId);
        _logger.LogInformation("Getting available coupons for user response. UserId: {UserId}", userId);
        var cart = await _cartValidation.ValidateGetCartItemsByUserId(userId);
        var cost = cart.Sum(c => c.Qunatity * c.ProductVariant!.Price);
        var productId = cart.Select(c => c.ProductVariant!.ProductId).Distinct().ToList();
        _logger.LogInformation("Calculated cart total: {CartTotal} with {ProductCount} distinct products for UserId: {UserId}", cost, productId.Count, userId);
        var coupons = await _couponValidation.ValidateGetAllAvailableCoupons(cost, productId, userId);
        _logger.LogInformation("Retrieved {CouponCount} available coupons for UserId: {UserId}", coupons.Count, userId);
        return _mapper.Map<List<ResponseGetAllCoupon>>(coupons);
    }
}