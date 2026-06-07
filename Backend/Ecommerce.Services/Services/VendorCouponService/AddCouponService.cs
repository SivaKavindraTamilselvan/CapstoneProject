using AutoMapper;
using Ecommerce.DTOs;
using Ecommerce.Models;
using Ecommerce.Repositories.Interfaces;
using Ecommerce.Services.Interfaces;

public class VendorCouponService : IVendorCouponService
{
    private readonly ICouponRepsository _couponRepsository;
    private readonly ICouponProductRepsository _couponProductRepsository;
    private readonly ICouponValidation _couponValidation;
    private readonly IMapper _mapper;
    public VendorCouponService(ICouponProductRepsository couponProductRepsository,ICouponRepsository couponRepsository,IMapper mapper,ICouponValidation couponValidation)
    {
        _couponProductRepsository = couponProductRepsository;
        _couponRepsository = couponRepsository;
        _couponValidation = couponValidation;
        _mapper = mapper;
    }

    public async Task<ResponseAddCouponDTO> AddCoupon(RequestAddCouponDTO requestAddCouponDTO,int roleId,int UserId)
    {
        await _couponValidation.ValidateCouponCode(requestAddCouponDTO.CouponCode);
        var coupon = _mapper.Map<Coupons>(requestAddCouponDTO);
        if(roleId == 1)
        {
            coupon.CouponTypeId = 1;
        }
        else
        {
            coupon.CouponTypeId = 2;
        }
        coupon.CreatedByUserId = UserId;
        await _couponRepsository.Create(coupon);
        return _mapper.Map<ResponseAddCouponDTO>(coupon);
    }
}