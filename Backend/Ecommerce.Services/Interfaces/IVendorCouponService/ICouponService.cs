using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorCouponService
{
    public Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO, int roleId, int UserId);
}