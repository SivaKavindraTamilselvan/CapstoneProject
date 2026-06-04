using Ecommerce.DTOs;
using Ecommerce.Models;

namespace Ecommerce.Services.Interfaces;

public interface IUserCouponService
{
    public Task<List<ResponseGetAllCoupon>> GetAllAvailableCoupons(int userId);
    public Task<List<ResponseGetAllCoupon>> GetAllActiveCoupons(int userId);
}