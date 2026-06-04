using Ecommerce.DTOs;

namespace Ecommerce.Services.Interfaces;

public interface IVendorCouponService
{
    public Task<ResponseAddCouponDTO> AddProductCoupon(RequestAddCouponProductDTO requestAddCouponProductDTO);
    public Task<ResponseAddCouponDTO> AddVendorCoupon(RequestAddCouponDTO requestAddCouponDTO,int vendorId);
    public Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO);
}