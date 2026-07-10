using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IUserCouponService
{
    public Task<List<ResponseGetAllCoupon>> GetAllAvailableCouponsUser(int userId);
    public Task<List<Coupons>> GetAllAvailableCoupons(int userId);
    public Task<List<ResponseGetAllCoupon>> GetAllActiveCoupons(int userId);
}