using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface ICouponService
{
    public Task<ResponseAddCouponDTO> ActivateCoupon(int couponId, int adminUserId);
    public Task<ResponseAddCouponDTO> DeactivateCoupon(int couponId, int adminUserId);
    public Task<ResponseAddCouponDTO> UpdateCouponByIdForAdmin(UpdateCouponDto update, int adminUserId);
    public Task<PagedResponse<CouponListDto>> GetCouponsForAdmin(AdminCouponFilter request, int adminUserId);
    public Task<CouponDetailDto> GetCouponByIdForAdmin(int couponId, int adminUserId);
    public Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO, int roleId, int UserId);
}